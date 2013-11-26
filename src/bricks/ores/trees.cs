datablock fxDTSBrickData(TreeMapleBrickData)
{
	brickFile = $BLS::Path @ "res/shapes/blb/Maple.blb";
	uiName = "Maple Tree";
	category = "Stranded";
	subCategory = "Resources";
	isTree = true;
	adminOnly = true;

	ore = "wood";
	spoils = "leaf branch bark";
	resources = 60;
	respawn = 10;

	iconName = $BLS::Path @ "res/shapes/blb/Icons/Maple.png";
};

datablock fxDTSBrickData(TreeOakBrickData)
{
	brickFile = $BLS::Path @ "res/shapes/blb/Oak.blb";
	uiName = "Oak Tree";
	category = "Stranded";
	subCategory = "Resources";
	isTree = true;
	adminOnly = true;

	ore = "wood";
	spoils = "leaf branch bark";
	resources = 60;
	respawn = 10;
	iconName = $BLS::Path @ "res/shapes/blb/Icons/Oak.png";
};

datablock fxDTSBrickData(TreePineBrickData)
{
	brickFile =$BLS::Path @ "res/shapes/blb/Pine.blb";
	uiName = "Pine_Tree";
	category = "Stranded";
	subCategory = "Resources";
	isTree = true;
	adminOnly = true;

	ore = "wood";
	spoils = "leaf branch bark";
	resources = 60;
	respawn = 10;

	iconName = $BLS::Path @ "res/shapes/blb/Icons/Pine.png";
};

datablock fxDTSBrickData(TreePineLargeBrickData)
{
	brickFile =$BLS::Path @ "res/shapes/blb/PineLarge.blb";
	uiName = "Large Pine Tree";
	category = "Stranded";
	subCategory = "Resources";
	isTree = true;
	adminOnly = true;

	ore = "wood";
	spoils = "leaf branch bark";
	resources = 60;
	respawn = 10;

	iconName = $BLS::Path @ "res/shapes/blb/Icons/PineLarge.png";
};

datablock fxDTSBrickData(TreeWillowBrickData)
{
	brickFile =$BLS::Path @ "res/shapes/blb/Willow.blb";
	uiName = "Willow Tree";
	category = "Stranded";
	subCategory = "Resources";
	isTree = true;
	adminOnly = true;

	ore = "wood";
	spoils = "leaf branch bark";
	resources = 60;
	respawn = 10;

	iconName = $BLS::Path @ "res/shapes/blb/Icons/Willow.png";
};

datablock fxDTSBrickData(TreePalmBrickData)
{
	brickFile =$BLS::Path @ "res/shapes/blb/Palm.blb";
	uiName = "Palm Tree";
	category = "Stranded";
	subCategory = "Resources";
	isTree = true;
	adminOnly = true;

	ore = "wood";
	spoils = "leaf branch bark";
	resources = 60;
	respawn = 10;

	iconName = $BLS::Path @ "res/shapes/blb/Icons/Palm.png";
};

datablock fxDTSBrickData(TreePalmLargeBrickData)
{
	brickFile =$BLS::Path @ "res/shapes/blb/PalmLarge.blb";
	uiName = "Large Palm Tree";
	category = "Stranded";
	subCategory = "Resources";
	isTree = true;
	adminOnly = true;

	ore = "wood";
	spoils = "leaf branch bark";
	resources = 60;
	respawn = 10;

	iconName = $BLS::Path @ "res/shapes/blb/Icons/PalmLarge.png";
};

datablock fxDTSBrickData(TreeDeadBrickData)
{
	brickFile =$BLS::Path @ "res/shapes/blb/Dead.blb";
	uiName = "Dead Tree";
	category = "Stranded";
	subCategory = "Resources";
	isTree = true;
	adminOnly = true;

	spoils = "branch bark";
	resources = 60;
	respawn = 10;

	iconName = $BLS::Path @ "res/shapes/blb/Icons/Dead.png";
};

datablock fxDTSBrickData(TreeDeadLargeBrickData)
{
	brickFile =$BLS::Path @ "res/shapes/blb/DeadLarge.blb";
	uiName = "Large Dead Tree";
	category = "Stranded";
	subCategory = "Resources";
	isTree = true;
	adminOnly = true;

	spoils = "branch bark";
	resources = 60;
	respawn = 10;

	iconName = $BLS::Path @ "res/shapes/blb/Icons/DeadLarge.png";
};

datablock fxDTSBrickData(TreeStumpBrickData)
{
	brickFile =$BLS::Path @ "res/shapes/blb/Stump.blb";
	uiName = "Stump";
	category = "Stranded";
	subCategory = "Resources";
	isTree = true;
	adminOnly = true;

	spoils = "branch bark";
	resources = 60;
	respawn = 10;

	iconName = $BLS::Path @ "res/shapes/blb/Icons/Stump.png";
};

function fxDTSBrick::onAxeHit( %this, %client )
{
	serverPlay3d(axeHitSound, %this.getPosition());
	%data = %this.getDatablock();
	if( !isObject( %client ) )
	{
		return;
	}

	if( %this.hasResources $= "" )
	{
		%this.hasResources = true;
	}

	if( !%this.hasResources )
	{
		commandToClient( %client, 'centerPrint', "\c6This tree has no resources!", 1 );
		return;
	}

	%rnd = getRandom( 0, 10 );
	if( %rnd > 7 && %data.ore !$= "" )
	{
		%amt = getRandom(1, 3);

		%ore = %data.ore;

		if(getWordCount(%ore) > 1) {
			%rnd = getRandom(0, getWordCount(%data.spoils) - 1);
			%ore = getWord(%data.ore, %rnd);
		}

		%this.resources -= 1;
		%client.resources[ %ore ] += %amt;
		// %client.updateInfo();

		commandToClient( %client, 'centerPrint', "<font:Courier:20>\c6+\c3" @ %amt SPC "\c6" SPC %ore @ "!", 0.5 );
	}
	else
	{
		%amt = getRandom(1, 3);
		%rnd = getRandom(0, getWordCount(%data.spoils) - 1);
		%ore = getWord(%data.spoils, %rnd);
		%client.resources[ %ore ] += %amt;
		commandToClient( %client, 'centerPrint', "<font:Courier:20>\c6+\c3" @ %amt SPC "\c6" SPC %ore @ "!", 0.5 );
	}

	if( %this.resources <= 0 )
	{
		%this.resources = %data.resources;
		%this.lastEmpty = $Sim::Time;
		%this.hasResources = false;
		%this.disappear(-1);
		%this.schedule(%data.respawn * 1000, treeRespawn );
	}
}

function fxDTSBrick::treeRespawn( %this )
{
	if(isObject(%this))
	{
		%this.disappear(0);
		%this.hasResources = true;
	}
}

package treeBrickpackage
{
	function fxDTSBrick::onPlant( %this )
	{
		parent::onPlant( %this );
		%this.schedule(0, handleTreeSpawn);
	}

	function fxDTSBrick::onLoadPlant( %this )
	{
		parent::onLoadPlant( %this );
		%this.schedule(0, handleTreeSpawn);
	}
};
activatePackage(oreBrickPackage);

function fxDTSBrick::handleTreeSpawn(%this) {
	if(%this.isTree) {
		announce("Tree!");
		%this.resources = %data.resources;
		%this.disappear(0);
		%this.hasResources = true;
	}
}