using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrentonDarts.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchTypeGameRuleDeletedAtAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "match_type_game_rules",
                type: "timestamp without time zone",
                nullable: true);

            // Seed one match type and its 19 game rules from the GTDL standard match format.
            // Only inserts if the match_types table is empty so re-running on an existing DB is safe.
            migrationBuilder.Sql(@"
DO $$
DECLARE mt_id INT;
BEGIN
  IF NOT EXISTS (SELECT 1 FROM match_types LIMIT 1) THEN
    INSERT INTO match_types (""Name"", created_at, updated_at)
    VALUES ('GTDL Standard', NOW(), NOW())
    RETURNING ""Id"" INTO mt_id;

    INSERT INTO match_type_game_rules
      (""matchTypeId"", ""gameType"", ""doubleIn"", ""doubleOut"", ""orderId"",
       ""bestOfNumberOfLegs"", ""numberOfLegs"", ""whoStarts"", ""numberOfPlayers"",
       ""gamePointValue"", ""legPointValue"", ""forfeitIfNoPlayers"", ""groupName"",
       created_at, updated_at)
    VALUES
      (mt_id,'cricket',      false,false, 1,0,1,'H',1,1,0,true, 'Cricket',        NOW(),NOW()),
      (mt_id,'cricket',      false,false, 2,0,1,'A',1,1,0,true, 'Cricket',        NOW(),NOW()),
      (mt_id,'cricket',      false,false, 3,0,1,'H',1,1,0,true, 'Cricket',        NOW(),NOW()),
      (mt_id,'doublecricket',false,false, 4,0,1,'A',2,2,0,true, 'Double Cricket', NOW(),NOW()),
      (mt_id,'501',          false,true,  5,0,1,'A',1,1,0,true, '501',            NOW(),NOW()),
      (mt_id,'501',          false,true,  6,0,1,'H',1,1,0,true, '501',            NOW(),NOW()),
      (mt_id,'501',          false,true,  7,0,1,'A',1,1,0,true, '501',            NOW(),NOW()),
      (mt_id,'double501',    false,true,  8,0,1,'H',2,2,0,true, 'Double 501',     NOW(),NOW()),
      (mt_id,'801',          false,true,  9,0,1,NULL,3,3,0,true,'801',            NOW(),NOW()),
      (mt_id,'doublecricket',false,false,10,0,1,'A',2,2,0,true, 'Double Cricket', NOW(),NOW()),
      (mt_id,'doublecricket',false,false,11,0,1,'H',2,2,0,true, 'Double Cricket', NOW(),NOW()),
      (mt_id,'double501',    false,true, 12,0,1,'A',2,2,0,true, 'Double 501',     NOW(),NOW()),
      (mt_id,'double501',    false,true, 13,0,1,'H',2,2,0,true, 'Double 501',     NOW(),NOW()),
      (mt_id,'cricket',      false,false,14,0,1,'A',1,1,0,true, 'Cricket',        NOW(),NOW()),
      (mt_id,'cricket',      false,false,15,0,1,'H',1,1,0,true, 'Cricket',        NOW(),NOW()),
      (mt_id,'cricket',      false,false,16,0,1,'A',1,1,0,true, 'Cricket',        NOW(),NOW()),
      (mt_id,'501',          false,true, 17,0,1,'H',1,1,0,true, '501',            NOW(),NOW()),
      (mt_id,'501',          false,true, 18,0,1,'A',1,1,0,true, '501',            NOW(),NOW()),
      (mt_id,'501',          false,true, 19,0,1,'H',1,1,0,true, '501',            NOW(),NOW());
  END IF;
END $$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "match_type_game_rules");
        }
    }
}
