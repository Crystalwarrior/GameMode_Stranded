//sounds

datablock audioProfile( fallDeathSound )
{
	fileName = $BLS::Path @ "res/sounds/fall.wav";
	description = audioClosest3D;
	preload = true;
};

datablock audioProfile( fallSound )
{
	fileName = $BLS::Path @ "res/sounds/fallsoft.wav";
	description = audioClosest3D;
	preload = true;
};

datablock audioProfile( jumpLandingSound )
{
	description = "audioClosest3D";
	fileName = $BLS::Path @ "res/sounds/jumplanding.wav";
	preload = true;
};

datablock audioProfile( pickaxeHitSound )
{
	fileName = $BLS::Path @ "res/sounds/pickaxe_hit.wav";
	description = audioClosest3D;
	preload = true;
};

datablock audioProfile( axeHitSound )
{
	fileName = $BLS::Path @ "res/sounds/axe_hit.wav";
	description = audioClosest3D;
	preload = true;
};

//default sound replacement
function addNewSounds()
{
	if(isFile($BLS::Path @ "res/sounds/jump.wav"))
		jumpSound.fileName = $BLS::Path @ "res/sounds/jump.wav";
		deathCrySound.fileName = "";
}

addnewSounds();

package BLS_SoundReplace
{
	function JumpSound::onAdd(%this)
	{
		parent::onAdd(%this);

		if(isFile($BLS::Path @ "res/sounds/jump.wav"))
			%this.fileName = $BLS::Path @ "res/sounds/jump.wav";
	}
};
activatePackage(BLS_SoundReplace);

//misc. shapes

//players

datablock PlayerData(StrandedPlayerArmor : PlayerStandardArmor)
{
	uiName = "Stranded Player";

	maxForwardSpeed = 7;
	maxBackwardSpeed = 4;
	maxSideSpeed = 3;
	maxForwardCrouchSpeed = 4;
	maxSideCrouchSpeed = 2;
	maxBackwardCrouchSpeed = 1;
	jumpForce = 12 * 90;
	//airControl = 0.02;
	airControl = 0.05;
	canJet = 0;

	minImpactSpeed = 19;
	speedDamageScale = 2.9;
	jumpDelay = 30;

	mass = 120;
};

datablock playerData(StrandedPlayerLimpingArmor : StrandedPlayerArmor)
{
	uiName = "Stranded Limping Player";
	maxForwardSpeed = 4;
	maxBackwardSpeed = 3;
	maxSideSpeed = 2;
	jumpForce = 9 * 90;
	airControl = 0.01;
	minImpactSpeed = 16;

	jumpDelay = 40;
};

datablock playerData(StrandedPlayerFrozenArmor : StrandedPlayerArmor)
{
	uiName = "";
	maxForwardSpeed = 0;
	maxBackwardSpeed = 0;
	maxSideSpeed = 0;
	maxForwardCrouchSpeed = 0;
	maxSideCrouchSpeed = 0;
	maxBackwardCrouchSpeed = 0;
	jumpForce = 0;
	airControl = 0;
};
