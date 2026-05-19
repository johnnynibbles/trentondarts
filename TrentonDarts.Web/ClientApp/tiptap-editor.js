import { Editor } from '@tiptap/core';
import StarterKit from '@tiptap/starter-kit';
import Link from '@tiptap/extension-link';
import Image from '@tiptap/extension-image';

const BUTTONS = [
  { label: 'B',  title: 'Bold',          style: 'font-weight:700', run: ed => ed.chain().focus().toggleBold().run(),                      isActive: ed => ed.isActive('bold') },
  { label: 'I',  title: 'Italic',        style: 'font-style:italic', run: ed => ed.chain().focus().toggleItalic().run(),                  isActive: ed => ed.isActive('italic') },
  { label: 'S',  title: 'Strikethrough', style: 'text-decoration:line-through', run: ed => ed.chain().focus().toggleStrike().run(),       isActive: ed => ed.isActive('strike') },
  null,
  { label: 'H2', title: 'Heading 2',     run: ed => ed.chain().focus().toggleHeading({ level: 2 }).run(), isActive: ed => ed.isActive('heading', { level: 2 }) },
  { label: 'H3', title: 'Heading 3',     run: ed => ed.chain().focus().toggleHeading({ level: 3 }).run(), isActive: ed => ed.isActive('heading', { level: 3 }) },
  null,
  { label: '•',  title: 'Bullet list',   run: ed => ed.chain().focus().toggleBulletList().run(),   isActive: ed => ed.isActive('bulletList') },
  { label: '1.', title: 'Ordered list',  run: ed => ed.chain().focus().toggleOrderedList().run(),  isActive: ed => ed.isActive('orderedList') },
  { label: '❝',  title: 'Blockquote',    run: ed => ed.chain().focus().toggleBlockquote().run(),   isActive: ed => ed.isActive('blockquote') },
  null,
  { label: '🔗', title: 'Link',          run: ed => {
      const prev = ed.isActive('link') ? ed.getAttributes('link').href : '';
      const url = window.prompt('URL', prev);
      if (url === null) return;
      if (url === '') { ed.chain().focus().unsetLink().run(); return; }
      ed.chain().focus().setLink({ href: url }).run();
    }, isActive: ed => ed.isActive('link') },
  null,
  { label: '↩', title: 'Undo', run: ed => ed.chain().focus().undo().run(), isActive: () => false },
  { label: '↪', title: 'Redo', run: ed => ed.chain().focus().redo().run(), isActive: () => false },
];

function imageUploadButton(uploadUrl) {
  return {
    label: '🖼', title: 'Insert image',
    run: (ed) => {
      const input = document.createElement('input');
      input.type = 'file';
      input.accept = 'image/*';
      input.onchange = async () => {
        const file = input.files[0];
        if (!file) return;
        const formData = new FormData();
        formData.append('file', file);
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        if (token) formData.append('__RequestVerificationToken', token);
        const res = await fetch(uploadUrl, { method: 'POST', body: formData });
        if (!res.ok) return;
        const { url } = await res.json();
        ed.chain().focus().setImage({ src: url }).run();
      };
      input.click();
    },
    isActive: () => false,
  };
}

function buildToolbar(editor, buttons) {
  const bar = document.createElement('div');
  bar.className = 'tiptap-toolbar';

  const tracked = [];

  for (const item of buttons) {
    if (item === null) {
      const sep = document.createElement('span');
      sep.className = 'tiptap-sep';
      bar.appendChild(sep);
      continue;
    }
    const btn = document.createElement('button');
    btn.type = 'button';
    btn.title = item.title;
    btn.textContent = item.label;
    if (item.style) btn.setAttribute('style', item.style);
    btn.className = 'tiptap-btn';
    btn.addEventListener('mousedown', e => { e.preventDefault(); item.run(editor); });
    bar.appendChild(btn);
    tracked.push({ btn, isActive: item.isActive });
  }

  function refresh() {
    for (const { btn, isActive } of tracked) {
      btn.classList.toggle('is-active', isActive(editor));
    }
  }
  editor.on('selectionUpdate', refresh);
  editor.on('transaction', refresh);

  return bar;
}

window.initTipTapEditor = function (containerId, hiddenInputId, initialHtml, options = {}) {
  const { imageUploadUrl } = options;
  const wrapper = document.getElementById(containerId);
  const hidden = document.getElementById(hiddenInputId);

  const mount = document.createElement('div');
  mount.className = 'tiptap-content';
  wrapper.appendChild(mount);

  const editor = new Editor({
    element: mount,
    extensions: [
      StarterKit,
      Link.configure({ openOnClick: false, autolink: true }),
      Image,
    ],
    content: initialHtml || '',
    onUpdate({ editor }) {
      hidden.value = editor.getHTML();
    },
  });

  const buttons = imageUploadUrl
    ? [...BUTTONS, null, imageUploadButton(imageUploadUrl)]
    : BUTTONS;

  wrapper.insertBefore(buildToolbar(editor, buttons), mount);

  wrapper.closest('form')?.addEventListener('submit', () => {
    hidden.value = editor.getHTML();
  });

  return editor;
};
