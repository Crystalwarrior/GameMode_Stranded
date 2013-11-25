package rpchat
{
	function serverCmdMessageSent(%client, %text) {
		if (!isObject(%client.miniGame.BLSCore) || %client.miniGame.BLSCore.ended) {
			Parent::serverCmdMessageSent(%client, %text);
			return;
		}

		if (getSimTime() - %client.lastChatTime < 500) {
			return;
		}

		%text = trim(stripMLControlChars(%text));

		if (%text $= "") {
			return;
		}

		%client.lastChatTime = getSimTime();
		echo(%client.getPlayerName() @ ":" SPC %text);
		
		if (isObject(%client.player) && %client.player.getState() !$= "Dead") {
			%range = 20;
			%action = "says";

			if (strLen(%text) > 1 && getSubStr(%text, 0, 1) $= "!") {
				%text = getSubStr(%text, 1, strLen(%text));
				%range = 50;
				%action = "yells";
			}
			else if (strLen(%text) > 1 && getSubStr(%text, 0, 1) $= "#") {
				%text = getSubStr(%text, 1, strLen(%text));
				%range = 5;
				%action = "whispers";
			}

			if (%action $= "says" && strLen(%text) > 1) {
				if (strLen(%text) > 2 && getSubStr(%text, strLen(%text) - 2, 2) $= "...") {
					%action = "mutters";
				}
				else if (getSubStr(%text, strLen(%text) - 1, 1) $= "!") {
					%action = "exclaims";

					if (strLen(%text) > 2 && getSubStr(%text, strLen(%text) - 2, 1) $= "!") {
						%range = 5;
						%action = "yells";
					}
				}
				else if (getSubStr(%text, strLen(%text) - 1, 1) $= "?") {
					%action = "asks";
				}
			}

			%message = "<color:FFFF66>" @ %client.getPlayerName() SPC "\c7" @ %action @ ", \c6'" @ %text @ "'";

			%client.player.playThread(0, "talk");
			%client.player.schedule(strLen(%text) * 35, playThread, 0, "root");

			initContainerRadiusSearch(%client.player.getHackPosition(), %range, $TypeMasks::PlayerObjectType);

			while (isObject(%obj = containerSearchNext())) {
				if (isObject(%obj.client)) {
					%heard[%obj.client] = true;
					chatMessageClient(%obj.client, "", "", "", %message);
				}
			}
		}
		else {
			%message = "\c7[DEAD]" SPC %client.getPlayerName() @ "\c6:" SPC %text;
		}

		%count = ClientGroup.getCount();

		for (%i = 0; %i < %count; %i++) {
			%current = ClientGroup.getObject(%i);

			if (!%heard[%current] && (%current.miniGame != %client.miniGame || !isObject(%current.player) || %current.player.getState() $= "Dead")) {
				chatMessageClient(%current, "", "", "", %message);
			}
		}
	}

	function serverCmdTeamMessageSent(%client, %text) {
		if (!isObject(%client.miniGame)) {
			Parent::serverCmdTeamMessageSent(%client, %text);
		}
	}
};
activatePackage(rpchat);