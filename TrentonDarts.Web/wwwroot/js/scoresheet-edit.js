(function(ko, $, postal){
    "strict";

    var channel = postal.channel();
    var token = document.querySelector("meta[name=csrf-token]").getAttribute("content");

    function ScoreSheetViewModel(awayPlayers, homePlayers, gameGroups, hasScorecard, awayScoreOverride, homeScoreOverride) {
        var self = this;
        self.saving = false;

        self.hasScorecard = ko.observable(hasScorecard);
        self.awayScoreOverride = ko.observable(awayScoreOverride);
        self.homeScoreOverride = ko.observable(homeScoreOverride);

        self.awayPlayers = awayPlayers;
        self.homePlayers = homePlayers;
        self.gameGroups = ko.observableArray(gameGroups);

        self.awayScore = ko.computed(function(){
            var score = 0;
            _.forEach(self.gameGroups(), function(group){
                _.forEach(group.games(), function(game){
                    score = score + game.awayScore();
                });
            });
            return score;
        });

        self.homeScore = ko.computed(function(){
            var score = 0;
            _.forEach(self.gameGroups(), function(group){
                _.forEach(group.games(), function(game){
                    score = score + game.homeScore();
                });
            });
            return score;
        });

        self.scoreAllHome = function() {
            _.forEach(self.gameGroups(), function(group) {
                _.forEach(group.games(), function(game) {
                    _.forEach(game.legs(), function(leg) { leg('home'); });
                });
            });
        };

        self.scoreAllAway = function() {
            _.forEach(self.gameGroups(), function(group) {
                _.forEach(group.games(), function(game) {
                    _.forEach(game.legs(), function(leg) { leg('away'); });
                });
            });
        };

        self.saveMatch = function() {
            if(self.saving) return;
            self.saving = true;

            var redirectUrl = scoreCardConfig.redirectUrl ||
                '/season/' + scoreCardConfig.seasonId + '/match/' + scoreCardConfig.matchId;

            var options = {
                url: '/season/' + scoreCardConfig.seasonId + '/match/' + scoreCardConfig.matchId,
                type: 'post',
                data: ko.toJSON(self),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                headers: { 'X-CSRF-Token': token }
            };
            $.ajax(options)
                .done(function() { window.location = redirectUrl; })
                .fail(function() {
                    alert('The score card failed to save.');
                    self.saving = false;
                });
        };

        self.cancelMatchEdit = function(){
            var redirectUrl = scoreCardConfig.redirectUrl ||
                '/season/' + scoreCardConfig.seasonId + '/match/' + scoreCardConfig.matchId;
            window.location = redirectUrl;
        };
    }

    function Game(id, gameType, numberOfPlayers, gameValue, numberOfLegs,
                  awayPlayersAll, homePlayersAll,
                  awayPlayer, homePlayer, awayPlayer2, homePlayer2, awayPlayer3, homePlayer3,
                  initialLegs, forfeitedBy) {
        var self = this;
        var nextInternalAwardId = -1;
        var selectedAwayPlayers = ko.observableArray([]),
            selectedHomePlayers = ko.observableArray([]),
            allAwayPlayers = awayPlayersAll,
            allHomePlayers = homePlayersAll;

        self.id = id;
        self.gameType = gameType;
        self.numberOfPlayers = numberOfPlayers;
        self.numberOfLegs = numberOfLegs || 1;
        self.forfeitedBy = forfeitedBy;

        function awayNewValue(newvalue){
            channel.publish('awayPlayer.changed', { id: self.id, gameType: self.gameType, newPlayer: newvalue });
        }
        function awayOldValue(oldvalue){
            channel.publish('awayPlayer.changed', { id: self.id, gameType: self.gameType, oldPlayer: oldvalue });
        }

        self.awayPlayer = ko.observable(awayPlayer || undefined);
        self.awayPlayer.subscribe(awayNewValue);
        self.awayPlayer.subscribe(awayOldValue, null, "beforeChange");
        self.awayPlayer2 = ko.observable(awayPlayer2 || undefined);
        self.awayPlayer2.subscribe(awayNewValue);
        self.awayPlayer2.subscribe(awayOldValue, null, "beforeChange");
        self.awayPlayer3 = ko.observable(awayPlayer3 || undefined);
        self.awayPlayer3.subscribe(awayNewValue);
        self.awayPlayer3.subscribe(awayOldValue, null, "beforeChange");

        self.awayPlayerMessage = ko.computed(function(){
            var players = [self.awayPlayer(), self.awayPlayer2(), self.awayPlayer3()];
            for(var i = 0; i < players.length; i++)
                for(var x = 0; x < players.length; x++){
                    if(x == i || players[i] === undefined || players[x] === undefined) continue;
                    if(players[i] === players[x]) return "Duplicate players.";
                }
            return "";
        });

        channel.subscribe('awayPlayer.changed', function(data){
            if(data.id === self.id) return;
            if(data.gameType == self.gameType){
                if(data.oldPlayer !== undefined) {
                    var oldId = ko.utils.arrayFirst(selectedAwayPlayers(), function(playerId){ return playerId === data.oldPlayer.id; });
                    selectedAwayPlayers.remove(oldId);
                }
                if(data.newPlayer !== undefined) selectedAwayPlayers.push(data.newPlayer.id);
            }
        });

        self.awayScoreable = ko.computed(function() {
            return self.awayPlayer() !== undefined || self.awayPlayer2() !== undefined || self.awayPlayer3() !== undefined;
        });

        function homeNewValue(newvalue){
            channel.publish('homePlayer.changed', { id: self.id, gameType: self.gameType, newPlayer: newvalue });
        }
        function homeOldValue(oldvalue){
            channel.publish('homePlayer.changed', { id: self.id, gameType: self.gameType, oldPlayer: oldvalue });
        }

        self.homePlayer = ko.observable(homePlayer || undefined);
        self.homePlayer.subscribe(homeNewValue);
        self.homePlayer.subscribe(homeOldValue, null, "beforeChange");
        self.homePlayer2 = ko.observable(homePlayer2 || undefined);
        self.homePlayer2.subscribe(homeNewValue);
        self.homePlayer2.subscribe(homeOldValue, null, "beforeChange");
        self.homePlayer3 = ko.observable(homePlayer3 || undefined);
        self.homePlayer3.subscribe(homeNewValue);
        self.homePlayer3.subscribe(homeOldValue, null, "beforeChange");

        self.homePlayerMessage = ko.computed(function(){
            var players = [self.homePlayer(), self.homePlayer2(), self.homePlayer3()];
            for(var i = 0; i < players.length; i++)
                for(var x = 0; x < players.length; x++){
                    if(x == i || players[i] === undefined || players[x] === undefined) continue;
                    if(players[i] === players[x]) return "Duplicate players.";
                }
            return "";
        });

        channel.subscribe('homePlayer.changed', function(data){
            if(data.id === self.id) return;
            if(data.gameType == self.gameType){
                if(data.oldPlayer !== undefined) {
                    var oldId = ko.utils.arrayFirst(selectedHomePlayers(), function(playerId){ return playerId === data.oldPlayer.id; });
                    selectedHomePlayers.remove(oldId);
                }
                if(data.newPlayer !== undefined) selectedHomePlayers.push(data.newPlayer.id);
            }
        });

        self.homeScoreable = ko.computed(function() {
            return self.homePlayer() !== undefined || self.homePlayer2() !== undefined || self.homePlayer3() !== undefined;
        });

        self.awayPlayers = ko.computed(function(){
            return ko.utils.arrayFilter(allAwayPlayers, function(player){
                return selectedAwayPlayers.indexOf(player.id) < 0;
            });
        });

        self.homePlayers = ko.computed(function(){
            return ko.utils.arrayFilter(allHomePlayers, function(player){
                return selectedHomePlayers.indexOf(player.id) < 0;
            });
        });

        // Per-leg observables — one per leg
        self.legs = ko.observableArray([]);
        var safeLegs = initialLegs || [];
        for (var i = 0; i < self.numberOfLegs; i++) {
            self.legs.push(ko.observable(safeLegs[i] || ''));
        }

        // winner derived from leg counts (read-only for multi-leg; driven by radio for single-leg)
        self.winner = ko.computed(function() {
            var homeWins = 0, awayWins = 0;
            _.forEach(self.legs(), function(leg) {
                var v = leg();
                if (v === 'home') homeWins++;
                if (v === 'away') awayWins++;
            });
            if (homeWins > awayWins) return 'home';
            if (awayWins > homeWins) return 'away';
            return '';
        });

        self.awayScore = ko.computed(function() {
            return self.winner() === 'away' ? gameValue : 0;
        });

        self.homeScore = ko.computed(function() {
            return self.winner() === 'home' ? gameValue : 0;
        });

        self.notifyPlayers = function(){
            if(self.awayPlayer() !== undefined)
                channel.publish('awayPlayer.changed', { id: self.id, gameType: self.gameType, newPlayer: self.awayPlayer() });
            if(self.awayPlayer2() !== undefined)
                channel.publish('awayPlayer.changed', { id: self.id, gameType: self.gameType, newPlayer: self.awayPlayer2() });
            if(self.awayPlayer3() !== undefined)
                channel.publish('awayPlayer.changed', { id: self.id, gameType: self.gameType, newPlayer: self.awayPlayer3() });
            if(self.homePlayer() !== undefined)
                channel.publish('homePlayer.changed', { id: self.id, gameType: self.gameType, newPlayer: self.homePlayer() });
            if(self.homePlayer2() !== undefined)
                channel.publish('homePlayer.changed', { id: self.id, gameType: self.gameType, newPlayer: self.homePlayer2() });
            if(self.homePlayer3() !== undefined)
                channel.publish('homePlayer.changed', { id: self.id, gameType: self.gameType, newPlayer: self.homePlayer3() });
        };

        self.awards = ko.observableArray([]);
        self.deletedAwards = [];

        self.newAward = function(){
            var modal = document.getElementById('awardModal');
            if (!modal) return;
            var form = document.getElementById('awardForm');
            var gameVm = new GameAward(nextInternalAwardId, self.id, 0, '', '', allAwayPlayers.concat(allHomePlayers), self);
            nextInternalAwardId--;
            ko.applyBindings(gameVm, form);
            modal.showModal();
        };

        self.addAward = function(gameAward){
            self.awards.push(gameAward);
        };

        self.deleteAward = function(item) {
            var index = self.awards.indexOf(item);
            self.awards.splice(index, 1);
            if(item.id > 0) self.deletedAwards.push(item);
        };
    }

    function GameGroup(name, games){
        var self = this;
        self.name = name;
        self.games = ko.observableArray(games);
        self.template = function(game){
            switch(game.numberOfPlayers) {
                case 1: return "gameSinglePlayer";
                case 2: return "gameDoublePlayer";
                case 3: return "gameTriplePlayer";
            }
            throw new Error("No template available for game with " + game.numberOfPlayers + " players.");
        };
    }

    function GameAward(awardId, gameId, playerId, awardType, awardValue, players, game){
        var self = this;
        self.id = awardId;
        self.gameId = gameId;
        self.gamePlayers = players;
        self.player = ko.observable(ko.utils.arrayFirst(players, function(player){ return player.id === playerId; }));
        self.awardTypes = ["High Out", "High On", "Round 9", "T 80", "T 77", "T 74", "T 71"];
        self.awardType = ko.observable(awardType);
        self.awardValue = ko.observable(awardValue);
        self.save = function(){
            game.addAward(self);
            document.getElementById('awardModal').close();
        };
    }

    $(function(){
        var awardModal = document.getElementById('awardModal');
        if (awardModal) {
            awardModal.addEventListener('close', function(){
                ko.cleanNode(document.getElementById('awardForm'));
            });
        }

        var awayPlayers = scoreCardConfig.awayPlayers;
        var homePlayers = scoreCardConfig.homePlayers;
        var currentGroupName = "";
        var groups = [];
        var games = [];

        _.forEach(scoreCardConfig.match.gameResults, function(gameResult) {
            if(currentGroupName === "") {
                currentGroupName = gameResult.gameRules.groupName;
            } else if(currentGroupName !== gameResult.gameRules.groupName) {
                groups.push(new GameGroup(currentGroupName, games));
                games = [];
                currentGroupName = gameResult.gameRules.groupName;
            }

            var awayPlayer = null, awayPlayer2 = null, awayPlayer3 = null,
                homePlayer = null, homePlayer2 = null, homePlayer3 = null;

            if(gameResult.awayPlayers[0])
                awayPlayer = ko.utils.arrayFirst(awayPlayers, function(p){ return p.id === gameResult.awayPlayers[0].id; });
            if(gameResult.awayPlayers[1])
                awayPlayer2 = ko.utils.arrayFirst(awayPlayers, function(p){ return p.id === gameResult.awayPlayers[1].id; });
            if(gameResult.awayPlayers[2])
                awayPlayer3 = ko.utils.arrayFirst(awayPlayers, function(p){ return p.id === gameResult.awayPlayers[2].id; });
            if(gameResult.homePlayers[0])
                homePlayer = ko.utils.arrayFirst(homePlayers, function(p){ return p.id === gameResult.homePlayers[0].id; });
            if(gameResult.homePlayers[1])
                homePlayer2 = ko.utils.arrayFirst(homePlayers, function(p){ return p.id === gameResult.homePlayers[1].id; });
            if(gameResult.homePlayers[2])
                homePlayer3 = ko.utils.arrayFirst(homePlayers, function(p){ return p.id === gameResult.homePlayers[2].id; });

            var game = new Game(
                gameResult.gameRules.id,
                gameResult.gameRules.gameType,
                gameResult.gameRules.numberOfPlayers,
                gameResult.gameRules.gamePointValue,
                gameResult.gameRules.numberOfLegs,
                awayPlayers,
                homePlayers,
                awayPlayer,
                homePlayer,
                awayPlayer2,
                homePlayer2,
                awayPlayer3,
                homePlayer3,
                gameResult.legs,
                gameResult.forfeitedBy);

            _.forEach(gameResult.awards, function(award){
                game.awards.push(new GameAward(
                    award.id, game.id,
                    award.player ? award.player.id : 0,
                    award.awardType, award.value,
                    awayPlayers.concat(homePlayers), game));
            });
            games.push(game);
        });

        if (games.length > 0) {
            groups.push(new GameGroup(currentGroupName, games));
        }

        var model = new ScoreSheetViewModel(
            awayPlayers,
            homePlayers,
            groups,
            scoreCardConfig.match.hasScorecard,
            scoreCardConfig.match.awayScoreOverride,
            scoreCardConfig.match.homeScoreOverride
        );

        ko.applyBindings(model, document.getElementById('scoresheet'));

        _.forEach(groups, function(group){
            _.forEach(group.games(), function(game){
                game.notifyPlayers();
            });
        });
    });
})(window.ko, window.jQuery, window.postal);
