$DAMAGE_ENUM_HEAD = 0;
$DAMAGE_ENUM_CHEST = 1;
$DAMAGE_ENUM_ARM = 2;
$DAMAGE_ENUM_LEG = 3;

package strandedDamagePackage {
	function Player::damage(%this, %sourceObject, %pos, %damage, %damageType) {
		if(%damage <= 0)
		{
			return;
		}

		%this.lastDamagePos = %pos;
		if (%this.isCorpse) {
			%this.doDripBlood();
			%this.doSplatterBlood(%pos, 2);
			createBloodExplosion(%pos, vectorNormalize(%this.getVelocity()), %this.getScale());
			return;
		}

		if (%this.dead || !isObject(%this.client.miniGame.strandedCore)) {
			Parent::damage(%this, %sourceObject, %pos, %damage, %damageType);
			return;
		}

		if (%damageType == $DamageType::Fall) {
			if (%this.isCrouched()) {
				%hit[$DAMAGE_ENUM_CHEST] = 1;
				%hit[$DAMAGE_ENUM_ARM] = 1;
			}
			else {
				%hit[$DAMAGE_ENUM_LEG] = 1;
			}

			%fell = true;
		}
		else {
			if(%this.isCrouched())
			{
				%damage = %damage * 3;
			}

			%hitBox = getHitBox(%sourceObject, %this, %pos);

			if (matchBodyArea(%hitBox, $headTest)) {
				%hit[$DAMAGE_ENUM_HEAD] = 1;
			}

			if (matchBodyArea(%hitBox, $chestTest)) {
				%hit[$DAMAGE_ENUM_CHEST] = 1;
			}

			if (matchBodyArea(%hitBox, $armTest)) {
				%hit[$DAMAGE_ENUM_ARM] = 1;
			}

			if (matchBodyArea(%hitBox, $legTest)) {
				%hit[$DAMAGE_ENUM_LEG] = 1;
			}

			if (%hit0 + %hit1 + %hit2 + %hit3 <= 0) {
				%hit[getRandom(0, 3)] = 1;
			}
		}

		%this.condition[$DAMAGE_ENUM_HEAD] += %damage * 1.3 * %hit[$DAMAGE_ENUM_HEAD];
		%this.condition[$DAMAGE_ENUM_CHEST] += %damage * %hit[$DAMAGE_ENUM_CHEST];
		%this.condition[$DAMAGE_ENUM_ARM] += %damage * %hit[$DAMAGE_ENUM_ARM];

		if (%hit[$DAMAGE_ENUM_LEG]) {
			if (%damageType != $DamageType::Fall) {
				%this.condition[$DAMAGE_ENUM_LEG] += %damage * 0.6;
			}
			else {
				%this.condition[$DAMAGE_ENUM_LEG] += %damage;
			}

			%dataBlock = %this.getDataBlock();

			if (%this.condition[$DAMAGE_ENUM_LEG] > 70 && %dataBlock != nameToID("StrandedLimpingArmor")) {
				%this.changeDatablock(StrandedLimpingArmor);
			}
			else if (%dataBlock != nameToID("StrandedArmor")) {
				%this.changeDatablock(StrandedArmor);
			}
		}

		%dataBlock = %this.getDataBlock();
		%this.overallDamage = 0;

		%this.overallDamage += %this.condition[$DAMAGE_ENUM_HEAD];
		%this.overallDamage += %this.condition[$DAMAGE_ENUM_CHEST];
		%this.overallDamage += %this.condition[$DAMAGE_ENUM_ARM];
		%this.overallDamage += %this.condition[$DAMAGE_ENUM_LEG];

		if (%this.damageType $= "") {
			%this.damageType = $DamageType_Array[%damageType];
		}

		createBloodExplosion(%pos, vectorNormalize(%this.getVelocity()), %this.getScale());
		%this.setDamageFlash(%this.getDamageFlash() + %this.overallDamage * 0.01);

		if (%this.overallDamage >= %dataBlock.maxDamage) {
			%this.dead = true;

			%this.doSplatterBlood();
			%this.startDrippingBlood(15);

			if (%damageType == $DamageType::Fall) {
				serverPlay3D(fallDeathSound, %this.getPosition());
			}

			%damage = %dataBlock.maxDamage * (1 + %this.isCrouched() * 2);

			if (%this.condition[$DAMAGE_ENUM_HEAD] < %dataBlock.maxDamage * 0.8 &&
				%damageType != $DamageType::Fall
			) {
				if (%dataBlock != nameToID("StrandedFrozenArmor")) {
					%this.changeDatablock(StrandedFrozenArmor);
				}

				%this.playThread(0, "sit");
				%this.schedule(1000, damage, %sourceObject, %pos, %damage, %damageType);

				serverPlay3d(deathSound @ getRandom(1, 3), %this.getEyePoint());
				schedule(800, 0, serverPlay3D, bodyFallSound, %this.getHackPosition());
			}
			else {
				Parent::damage(%this, %sourceObject, %pos, %damage, %damageType);
			}
		}
		else {
			if (%damageType == $DamageType::Fall) {
				serverPlay3D(fallSound, %this.getPosition());
			}

			if ($Sim::Time - %this.lastPainCry >= 0.5) {
				serverPlay3D(painSound @ getRandom(1, 4), %this.getEyePoint());
				%this.lastPainCry = $Sim::Time;
			}

			%time = (%this.overallDamage / (%dataBlock.maxDamage / 4)) * 5;
			%time = mClampF(%time, 0, 10);

			%this.startDrippingBlood(%time);
		}
	}
};

activatePackage("strandedDamagePackage");


function Player::startSappingHealth(%this, %damage, %delay, %duration) {
	if(%damage == 0) {
		return;
	}

	%duration = mClampF(%duration, 0, 300);
	%remaining = %this.sapHealthEndTime - $Sim::Time;

	if (%duration == 0 || (%this.sapHealthEndTime !$= "" && %duration < %remaining)) {
		return;
	}

	%this.sapHealthEndTime = $Sim::Time + %duration;

	if(%delay <= 0) {
		%delay = 16;
	}

	if (!isEventPending(%this.sapHealthSchedule)) {
		%this.sapHealthSchedule = %this.schedule(%delay, sapHealthSchedule, %damage, %delay, %duration);
	}
}

function player::sapHealthSchedule(%this, %damage, %delay, %duration) {
	cancel(%this.sapHealth);

	if(!isObject(%this) || %this.getState() $= "Dead") {
		return;
	}

	if ($Sim::Time >= %this.sapHealthEndTime) {
		return;
	}

	%this.damage(%this, "", %damage, $DamageType::Direct);

	%this.sapHealthSchedule = %this.schedule(%delay, sapHealthSchedule, %damage, %delay, %duration);
}

function serverCmdHealth(%client) {
	%obj = %client.player;

	if (!isObject(%obj) || %obj.getState() $= "Dead") {
		return;
	}

	if (%obj.condition[$DAMAGE_ENUM_HEAD] <= 0) {
		messageClient(%client, '', "\c2My head is okay.");
	}
	else if (%obj.condition[$DAMAGE_ENUM_HEAD] <= 50) {
		messageClient(%client, '', "\c3My head is bruised.");
	}
	else {
		messageClient(%client, '', "\c0My head is bleeding.");
	}

	if (%obj.condition[$DAMAGE_ENUM_CHEST] <= 0) {
		messageClient(%client, '', "\c2My chest is okay.");
	}
	else if (%obj.condition[$DAMAGE_ENUM_CHEST] <= 50) {
		messageClient(%client, '', "\c3My chest is bruised.");
	}
	else {
		messageClient(%client, '', "\c0My chest is bleeding.");
	}

	if (%obj.condition[$DAMAGE_ENUM_ARM] <= 0) {
		messageClient(%client, '', "\c2My arms are okay.");
	}
	else if (%obj.condition[$DAMAGE_ENUM_ARM] <= 50) {
		messageClient(%client, '', "\c3My arms are bruised.");
	}
	else {
		messageClient(%client, '', "\c0My arms are bleeding.");
	}

	if (%obj.condition[$DAMAGE_ENUM_LEG] <= 0) {
		messageClient(%client, '', "\c2My legs are okay.");
	}
	else if (%obj.condition[$DAMAGE_ENUM_LEG] <= 50) {
		messageClient(%client, '', "\c3My legs are bruised.");
	}
	else {
		messageClient(%client, '', "\c0My legs are bleeding.");
	}
}