datablock ExplosionData(BLSPickaxeExplosion)
{
	lifeTimeMS = 400;

	soundProfile = ""; //pickaxeHitSound;

	particleEmitter = swordExplosionEmitter;
	particleDensity = 10;
	particleRadius = 0.2;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "10.0 11.0 10.0";
	camShakeAmp = "0.5 0.5 0.5";
	camShakeDuration = 0.25;
	camShakeRadius = 5.0;

	lightStartRadius = 1.5;
	lightEndRadius = 0;
	lightStartColor = "00.0 0.2 0.6";
	lightEndColor = "0 0 0";
};


datablock ItemData(BLSPickaxeItem : swordItem)
{
	shapeFile = $BLS::Path @ "res/Shapes/Pickaxe/Lil_Pickaxe.dts";
	uiName = "Pickaxe";
	doColorShift = true;
	colorShiftColor = "0.54 0.27 0.07 1";

	image = BLSPickaxeImage;
	canDrop = true;
	iconName = $BLS::Path @ "res/Shapes/Pickaxe/icon_RPGPickaxe";
};

AddDamageType("BLSAxe",   '<bitmap:Add-Ons/Gamemode_Stranded/res/Shapes/pickaxe/CI_RPGPickaxe> %1',    '%2 <bitmap:Add-Ons/Gamemode_Stranded/res/Shapes/pickaxe/CI_RPGPickaxe> %1',0.75,1);

datablock ProjectileData(BLSPickaxeProjectile)
{
	directDamage        = 0;
	directDamageType  = $DamageType::BLSPickaxe;
	radiusDamageType  = $DamageType::BLSPickaxe;
	explosion           = BLSPickaxeExplosion;

	muzzleVelocity      = 50;
	velInheritFactor    = 1;

	armingDelay         = 0;
	lifetime            = 100;
	fadeDelay           = 70;
	bounceElasticity    = 0;
	bounceFriction      = 0;
	isBallistic         = false;
	gravityMod = 0.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "";
};



datablock ShapeBaseImageData(BLSPickaxeImage)
{
	shapeFile = BLSPickaxeItem.shapeFile;
	emap = true;

	mountPoint = 0;
	offset = "0 0 0";

	correctMuzzleVector = false;

	className = "WeaponImage";

	item = BLSPickaxeItem;
	ammo = " ";
	projectile = BLSPickaxeProjectile;
	projectileType = Projectile;


	melee = true;
	doRetraction = false;

	armReady = true;


	doColorShift = true;
	colorShiftColor = "0.54 0.27 0.07 1";

	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.5;
	stateTransitionOnTimeout[0]      = "Ready";
	stateSound[0]                    = weaponSwitchSound;

	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "PreFire";
	stateAllowImageChange[1]         = true;

	stateName[2]			= "PreFire";
	stateScript[2]                  = "onPreFire";
	stateAllowImageChange[2]        = false;
	stateTimeoutValue[2]            = 0.2;
	stateTransitionOnTimeout[2]     = "Fire";

	stateName[3]                    = "Fire";
	stateTransitionOnTimeout[3]     = "CheckFire";
	stateTimeoutValue[3]            = 0.5;
	stateFire[3]                    = true;
	stateAllowImageChange[3]        = false;
	stateSequence[3]                = "Fire";
	stateScript[3]                  = "onFire";
	stateWaitForTimeout[3]		= true;

	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "PreFire";

	
	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Ready";
	stateTimeoutValue[5]            = 0.1;
	stateAllowImageChange[5]        = false;
	stateWaitForTimeout[5]		= true;
	stateSequence[5]                = "StopFire";
	stateScript[5]                  = "onStopFire";


};

function BLSPickaxeImage::onPreFire(%this, %obj, %slot)
{
	%obj.playthread(2, shiftAway);
}

function BLSPickaxeImage::onFire(%this, %obj, %slot)
{	
	%obj.playthread(2, shiftTo);
	parent::onFire(%this, %obj, %slot);
}

function BLSPickaxeProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
{
	parent::onCollision(%this,%obj,%col,%fade,%pos,%normal);
	if(%col.getClassName() $= "fxDTSBrick" && %col.GetDataBlock().isOre)
	{
		%col.onPickaxeHit(%obj.client);
	}
	else
	{
		serverPlay3D(hammerHitSound, %pos);
	}
}