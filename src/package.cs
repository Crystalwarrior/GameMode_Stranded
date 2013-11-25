package BLSPackage {
	function MiniGameSO::addMember(%this, %client) {
		Parent::addMember(%this, %client);
		if (!isObject(%this.BLSCore)) {
			BLSCore(%this);
		}
	}

	function MiniGameSO::removeMember(%this, %client) {
		Parent::removeMember(%this, %client);
	}

	function MiniGameSO::reset(%this, %client) {
		Parent::reset(%this, %client);

		if (isObject(DecalGroup)) {
			DecalGroup.deleteAll();
		}

		if (isObject(%this.BLSCore)) {
			%this.BLSCore.delete();
			%existed = true;
		}
		
		%this.BLSCore = BLSCore(%this);
	}

	function GameConnection::spawnPlayer(%this) {
		Parent::spawnPlayer(%this);

		if (!isObject(%this.miniGame.BLSCore) || !isObject(%this.player)) {
			announce("fail");
			return;
		}

		%this.player.setShapeNameColor("1 1 1");
		%this.player.setShapeNameDistance(15);
	}

	function GameConnection::onDeath(%this, %sourcePlayer, %sourceClient, %damageType, %damageArea) {
		serverCmdDropTool(%this, %this.player.currTool);
		Parent::onDeath(%this, %sourcePlayer, %sourceClient, %damageType, %damageArea);

		clearCenterPrint(%this);
		messageClient(%this, 'MsgYourSpawn');
	}

	function GameConnection::setControlObject(%this, %obj) {
		Parent::setControlObject(%this, %obj);
	}

	function Armor::damage(%this, %obj, %src, %pos, %type) {
		Parent::damage(%this, %obj, %src, %pos, %type);
	}

	function serverCmdDropTool(%client, %slot) {
		$DropToolClient = %client;
		Parent::serverCmdDropTool(%client, %slot);
		$DropToolClient = -1;
	}

	function ItemData::onAdd(%this, %obj) {
		Parent::onAdd(%this, %obj);

		if (isObject($DropToolClient)) {
			%obj.client = $DropToolClient;
		}
	}

	function Item::fadeOut(%this) {
		Parent::fadeOut(%this);
	}
};

activatePackage("BLSPackage");