datablock ExplosionData(BLSAxeExplosion)
{
	lifeTimeMS = 300;

	soundProfile = ""; //axeHitSound;

	particleEmitter = swordExplosionEmitter;
	particleDensity = 8;
	particleRadius = 0.2;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "12.0 14.0 12.0";
	camShakeAmp = "0.7 0.7 0.7";
	camShakeDuration = 0.35;
	camShakeRadius = 7.0;

	lightStartRadius = 1.5;
	lightEndRadius = 0;
	lightStartColor = "00.0 0.2 0.6";
	lightEndColor = "0 0 0";
};

datablock ItemData(BLSAxeItem : swordItem)
{
	shapeFile = $BLS::Path @ "res/Shapes/Axe/Lil_Axe.dts";
	uiName = "Axe";
	doColorShift = true;
	colorShiftColor = "0.54 0.27 0.07 1";

	image = BLSAxeImage;
	canDrop = true;
	iconName = $BLS::Path @ "res/Shapes/Axe/icon_RPGAxe";

	toolType = "axe";
};

AddDamageType("BLSAxe",   '<bitmap:Add-Ons/Gamemode_Stranded/res/Shapes/axe/CI_RPGAxe> %1',    '%2 <bitmap:Add-Ons/Gamemode_Stranded/res/Shapes/axe/CI_RPGAxe> %1',0.75,1);

datablock ProjectileData(BLSAxeProjectile)
{
	directDamage      = 0;
	directDamageType  = $DamageType::BLSAxe;
	radiusDamageType  = $DamageType::BLSAxe;
	explosion         = BLSAxeExplosion;

	muzzleVelocity      = 65;
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

	uiName = "BLS Axe Hit";
};

datablock ShapeBaseImageData(BLSAxeImage)
{
	shapeFile = BLSAxeItem.shapeFile;
	emap = true;

	mountPoint = 0;
	offset = "0 0 0";

	correctMuzzleVector = false;

	className = "WeaponImage";

	item = BLSAxeItem;
	ammo = " ";
	projectile = BLSAxeProjectile;
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
	stateTimeoutValue[2]            = 0.1;
	stateTransitionOnTimeout[2]     = "Fire";

	stateName[3]                    = "Fire";
	stateTransitionOnTimeout[3]     = "CheckFire";
	stateTimeoutValue[3]            = 0.2;
	stateFire[3]                    = true;
	stateAllowImageChange[3]        = false;
	stateSequence[3]                = "Fire";
	stateScript[3]                  = "onFire";
	stateWaitForTimeout[3]		= true;

	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "Fire";

	
	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Ready";
	stateTimeoutValue[5]            = 0.1;
	stateAllowImageChange[5]        = false;
	stateWaitForTimeout[5]		= true;
	stateSequence[5]                = "StopFire";
	stateScript[5]                  = "onStopFire";


};

function BLSAxeImage::onPreFire(%this, %obj, %slot)
{
	%obj.playthread(2, armattack);
}

function BLSAxeImage::onStopFire(%this, %obj, %slot)
{	
	%obj.playthread(2, root);
}

function BLSAxeProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
{
	parent::onCollision(%this,%obj,%col,%fade,%pos,%normal);
	if(%col.getClassName() $= "fxDTSBrick" && %col.GetDataBlock().isTree)
	{
		%col.onAxeHit(%obj.client);
	}
	else
	{
		serverPlay3D(swordHitSound, %pos);
	}
}