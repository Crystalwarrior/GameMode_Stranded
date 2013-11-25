function BLSCore(%miniGame) {
	if (!isObject(%miniGame)) {
		error("ERROR: Invalid mini-game specified.");
		return;
	}
	return new ScriptObject() {
		class = BLSCore;
		miniGame = %miniGame;
	};
}

function BLSCore::onAdd(%this) {
	%this.startTime = $Sim::Time;

	if (isObject(DayCycle)) {
		%time = $Sim::Time / DayCycle.dayLength;
		DayCycle.setDayOffset(0 - (%time - mCeil(%time)));
	}
}

function BLSCore::onRemove(%this) {
	for (%i = 0; %i < %this.miniGame.numMembers; %i++) {
		%this.miniGame.member[%i].isBLS = "";
	}
}

function BLSCore::end(%this, %message) {
	//a
}