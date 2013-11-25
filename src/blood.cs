$Blood::DripTimeOnBlood = 15;
$Blood::DripTimeOnProjectile = 5;
$Blood::DripTimeOnDamage = 5;

function BloodProjectile::onCollision(%this, %obj, %col, %pos, %fade, %normal) {
	Parent::onCollision(%this, %obj, %col, %pos, %fade, %normal);

	if (%obj.getType() & $TypeMasks::FxBrickObjectType) {
		spawnDecal(BloodDecal @ getRandom(1, 2), vectorSub(%pos, vectorScale(%normal, 0.2)), %normal);
	}

	if (%obj.getType() & $TypeMasks::PlayerObjectType) {
		%obj.startDrippingBlood($Blood::DripTimeOnBlood);
	}
}

function createBloodProjectile(%position, %velocity, %size) {
	%obj = new Projectile() {
		dataBlock = BloodProjectile;

		initialPosition = %position;
		initialVelocity = %velocity;
	};

	MissionCleanup.add(%obj);

	if (%size !$= "") {
		%obj.setScale(%size SPC %size SPC %size);
	}

	return %obj;
}

function Player::startDrippingBlood(%this, %duration) {
	%duration = mClampF(%duration, 0, 60);
	%remaining = %this.dripBloodEndTime - $Sim::Time;

	if (%duration == 0 || (%this.dripBloodEndTime !$= "" && %duration < %remaining)) {
		return;
	}

	%this.dripBloodEndTime = $Sim::Time + %duration;

	if (!isEventPending(%this.dripBloodSchedule)) {
		%this.dripBloodSchedule = %this.schedule(getRandom(300, 800), dripBloodSchedule);
	}
}

function Player::stopDrippingBlood(%this) {
	%this.dripBloodEndTime = "";
	cancel(%this.dripBloodSchedule);
}

function Player::dripBloodSchedule(%this) {
	cancel(%this.dripBloodSchedule);

	if ($Sim::Time >= %this.dripBloodEndTime) {
		return;
	}

	%this.doDripBlood(true);
	%this.dripBloodSchedule = %this.schedule(getRandom(300, 800), dripBloodSchedule);
}

function Player::doDripBlood(%this, %force) {
	if (!%force && $Sim::Time - %this.lastBloodDrip <= 0.2) {
		return false;
	}

	%ray = containerRayCast(
		vectorAdd(%this.position, "0 0 0.1"),
		vectorSub(%this.position, "0 0 0.1"),
		$TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType
	);

	%this.lastBloodDrip = $Sim::Time;
	spawnDecalFromRayCast(BloodDecal @ getRandom(1, 2), %ray, 0.4 + getRandom() * 0.85);

	return true;

	%this.lastBloodDrip = $Sim::Time;

	%x = getRandom() * 6 - 3;
	%y = getRandom() * 6 - 3;
	%z = 0 - (20 + getRandom() * 40);

	createBloodProjectile(%this.getHackPosition(), %x SPC %y SPC %z);
	return true;
}

function Player::doSplatterBlood(%this, %pos, %amount) {
	if (%pos $= "") {
		%pos = %this.getHackPosition();
	}

	if (%amount $= "") {
		%amount = getRandom(15, 30);
	}

	%masks = $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType;
	%spread = 0.25 + getRandom() * 0.25;

	for (%i = 0; %i < %amount; %i++) {
		%cross = vectorScale(vectorSpread("0 0 -1", %spread), 6);
		%stop = vectorAdd(%pos, %cross);

		%ray = containerRayCast(%pos, %stop, %masks);
		%scale = 0.4 + getRandom() * 0.85;
		spawnDecalFromRaycast(BloodDecal @ getRandom(1, 2), %ray, %scale);
		createBloodExplosion(getWords(%ray, 1, 3), vectorNormalize(%this.getVelocity()), %scale SPC %scale SPC %scale);
	}
}

function doSprayBlood(%pos, %vector, %amount) {
	if (%pos $= "") {
		return;
	}

	if (%amount $= "") {
		%amount = getRandom(5, 15);
	}

	%masks = $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType;
	%spread = 0.001 + getRandom() * 0.001;

	for (%i = 0; %i < %amount; %i++) {
		%cross = %vector;
		%stop = vectorAdd(%pos, %cross);

		%ray = containerRayCast(%pos, %stop, %masks);
		%scale = 0.4 + getRandom() * 0.85;
		spawnDecalFromRaycast(BloodDecal @ getRandom(1, 2), %ray, %scale);
		createBloodExplosion(getWords(%ray, 1, 3), vectorNormalize(%this.getVelocity()), %scale SPC %scale SPC %scale);
	}
}

function createBloodExplosion(%position, %velocity, %scale) {
	%datablock = bloodExplosionProjectile @ getRandom(1, 2);
	%obj = new Projectile() {
		dataBlock = %datablock;

		initialPosition = %position;
		initialVelocity = %velocity;
	};

	MissionCleanup.add(%obj);

	%obj.setScale(%scale);
	%obj.explode();
}

package BloodPackage {
	function Projectile::damage(%this, %obj, %col, %pos, %normal, %c) {
		Parent::damage(%this, %obj, %col, %pos, %normal, %c);
	}

	function Armor::damage(%this, %obj, %source, %amount, %origin, %type) {
		Parent::damage(%this, %obj, %source, %amount, %origin, %type);
	}

	function Armor::onDisabled(%this, %obj, %enabled) {
		Parent::onDisabled(%this, %obj, %enabled);
	}
};

activatePackage("BloodPackage");

datablock staticShapeData( BloodDecal1 )
{
	shapeFile = $BLS::Path @ "res/shapes/blood1_decal.dts";
	doColorShift = true;
	colorShiftColor = "0.7 0 0 1";
};

datablock staticShapeData( BloodDecal2 )
{
	shapeFile = $BLS::Path @ "res/shapes/blood2_decal.dts";
	doColorShift = true;
	colorShiftColor = "0.7 0 0 1";
};
datablock ParticleData(bloodParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.2;
	gravityCoefficient	= 0.2;
	inheritedVelFactor	= 1;
	constantAcceleration	= 0.0;
	lifetimeMS		= 500;
	lifetimeVarianceMS	= 10;
	spinSpeed		= 40.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= $BLS::Path @ "res/shapes/blood2.png";
	//animTexName		= " ";

	// Interpolation variables
	colors[0]	= "0.7 0 0 1";
	colors[1]	= "0.7 0 0 0";
	sizes[0]	= 0.4;
	sizes[1]	= 2;
	//times[0]	= 0.5;
	//times[1]	= 0.5;
};

datablock ParticleEmitterData(bloodEmitter)
{
	ejectionPeriodMS = 3;
	periodVarianceMS = 0;

	ejectionVelocity = 0; //0.25;
	velocityVariance = 0; //0.10;

	ejectionOffset = 0;

	thetaMin         = 0.0;
	thetaMax         = 90.0;  

	particles = bloodParticle;

	useEmitterColors = true;
	uiName = "";
};

datablock ExplosionData(bloodExplosion)
{
	//explosionShape = "";
	//soundProfile = bulletHitSound;
	lifeTimeMS = 300;

	particleEmitter = bloodEmitter;
	particleDensity = 5;
	particleRadius = 0.2;
	//emitter[0] = bloodEmitter;

	faceViewer     = true;
	explosionScale = "1 1 1";
};

datablock ProjectileData(bloodExplosionProjectile1)
{
	directDamage        = 0;
	impactImpulse	     = 0;
	verticalImpulse	  = 0;
	explosion           = bloodExplosion;
	particleEmitter     = bloodEmitter;

	muzzleVelocity      = 50;
	velInheritFactor    = 1;

	armingDelay         = 0;
	lifetime            = 2000;
	fadeDelay           = 1000;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.20;
	isBallistic         = true;
	gravityMod = 0.1;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
};



datablock ParticleData(bloodParticle2)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.1;
	gravityCoefficient	= 0.3;
	inheritedVelFactor	= 1;
	constantAcceleration	= 0.0;
	lifetimeMS		= 300;
	lifetimeVarianceMS	= 10;
	spinSpeed		= 20.0;
	spinRandomMin		= -10.0;
	spinRandomMax		= 10.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= $BLS::Path @ "res/shapes/blood3.png";
	//animTexName		= " ";

	// Interpolation variables
	colors[0]	= "0.7 0 0 1";
	colors[1]	= "0.7 0 0 0";
	sizes[0]	= 1;
	sizes[1]	= 0;
	//times[0]	= 0.5;
	//times[1]	= 0.5;
};

datablock ParticleEmitterData(bloodEmitter2)
{
	ejectionPeriodMS = 5;
	periodVarianceMS = 0;

	ejectionVelocity = 0; //0.25;
	velocityVariance = 0; //0.10;

	ejectionOffset = 0;

	thetaMin         = 0.0;
	thetaMax         = 90.0;  

	particles = bloodParticle2;

	useEmitterColors = true;
	uiName = "";
};

datablock ExplosionData(bloodExplosion2)
{
	//explosionShape = "";
	//soundProfile = bulletHitSound;
	lifeTimeMS = 300;

	particleEmitter = bloodEmitter2;
	particleDensity = 5;
	particleRadius = 0.2;
	//emitter[0] = bloodEmitter;

	faceViewer     = true;
	explosionScale = "1 1 1";
};

datablock ProjectileData(bloodExplosionProjectile2)
{
	directDamage        = 0;
	impactImpulse	     = 0;
	verticalImpulse	  = 0;
	explosion           = bloodExplosion2;
	particleEmitter     = bloodEmitter2;

	muzzleVelocity      = 50;
	velInheritFactor    = 1;

	armingDelay         = 0;
	lifetime            = 2000;
	fadeDelay           = 1000;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.20;
	isBallistic         = true;
	gravityMod = 0.1;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
};