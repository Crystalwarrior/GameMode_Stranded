package StrandedExaminePackage {
	function Player::activateStuff(%this) {
		Parent::activateStuff(%this);

		if (!isObject(%this.client)) {
			return;
		}

		%eyePoint = %this.getEyePoint();
		%eyeVector = %this.getEyeVector();

		%ray = containerRayCast(%eyePoint,
			vectorAdd(%eyePoint, vectorScale(%eyeVector, 6)),
			$TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType,
			%this
		);

		%col = firstWord(%ray);

		if (!isObject(%col) || !(%col.getType() & $TypeMasks::PlayerObjectType)) {
			return;
		}

		%text = %this.client.getStrandedExamineText(%col);

		if (%text !$= "") {
			%this.client.centerPrint(%text, 3);
		}
	}
};

activatePackage("StrandedExaminePackage");

function GameConnection::getStrandedExamineText(%this, %obj) {
	if (isObject(%obj.client)) {
		%message = "\c6This is \c3" @ %obj.client.getPlayerName() @ "\c6.\n";
	}

	%image = %obj.getMountedImage(0);

	if (isObject(%image) && nameToID(%image.item.image) == %image && %image.item.uiName !$= "") {
		%message = %message @ "\c6They are wielding a(n)" SPC %image.item.uiName @ ".\n";
	}

	if (%message !$= "") {
		%message = %message @ "\n";
	}

	if (%obj.condition[$DAMAGE_ENUM_HEAD] <= 0) {
		%message = %message @ "\c2Their head is okay.\n";
	}
	else if (%obj.condition[$DAMAGE_ENUM_HEAD] <= 50) {
		%message = %message @ "\c3Their head is bruised.\n";
	}
	else {
		%message = %message @ "\c0Their head is bleeding.\n";
	}

	if (%obj.condition[$DAMAGE_ENUM_CHEST] <= 0) {
		%message = %message @ "\c2Their chest is okay.\n";
	}
	else if (%obj.condition[$DAMAGE_ENUM_CHEST] <= 50) {
		%message = %message @ "\c3Their chest is bruised.\n";
	}
	else {
		%message = %message @ "\c0Their chest is bleeding.\n";
	}

	if (%obj.condition[$DAMAGE_ENUM_ARM] <= 0) {
		%message = %message @ "\c2Their arms are okay.\n";
	}
	else if (%obj.condition[$DAMAGE_ENUM_ARM] <= 50) {
		%message = %message @ "\c3Their arms are bruised.\n";
	}
	else {
		%message = %message @ "\c0Their arms are bleeding.\n";
	}

	if (%obj.condition[$DAMAGE_ENUM_LEG] <= 0) {
		%message = %message @ "\c2Their legs are okay.";
	}
	else if (%obj.condition[$DAMAGE_ENUM_LEG] <= 50) {
		%message = %message @ "\c3Their legs are bruised.";
	}
	else {
		%message = %message @ "\c0Their legs are bleeding.";
	}

	return %message;
}