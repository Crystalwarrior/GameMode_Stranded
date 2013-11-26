datablock fxDTSBrickData( oreBrickData : brick4xCubeData )
{
	brickFile = $BLS::Path @ "res/shapes/blb/rock.blb";
	//iconName = $BLS::Path @ "res/shapes/blb/4x Cube.png";

	category = "Stranded";
	subCategory = "Resources";
	
	uiName = "Random Ore";

	isOre = true;
	adminOnly = true;
	ore = "iron gold copper";
	spoils = "rock pebbles";
	resources = 60;
	respawn = 10;
};

datablock staticShapeData( oreShapeData )
{
	shapeFile = $BLS::Path @ "res/shapes/rock_ores.dts";
};

function fxDTSBrick::onPickaxeHit( %this, %client )
{
	serverPlay3d(pickaxeHitSound, %this.getPosition());
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
		commandToClient( %client, 'centerPrint', "\c6This rock has no resources!", 1 );
		return;
	}

	%rnd = getRandom( 0, 10 );
	if( %rnd >= 8 )
	{
		%amt = 1;

		%this.resources -= 1;
		%client.resources[ %this.ore ] += %amt;
		%client.updateInfo();

		commandToClient( %client, 'centerPrint', "<font:Courier:20>\c6+\c3" @ %amt SPC "\c6" SPC %this.ore @ "!", 0.5 );
	}
	else
	{
		%data = %this.getDatablock();
		%amt = getRandom(1, 3);
		%rnd = getRandom(0, getWordCount(%data.spoils) - 1);
		%ore = getWord(%data.spoils, %rnd);
		%client.resources[ %ore ] += %amt;
		commandToClient( %client, 'centerPrint', "<font:Courier:20>\c6+\c3" @ %amt SPC "\c6" SPC %ore @ "!", 0.5 );
	}

	if( %this.resources <= 0 )
	{
		%this.resources = %this.getDatablock().resources;
		%this.lastEmpty = $Sim::Time;
		%this.hasResources = false;
		%this.oreShape.delete();
		%this.schedule(%this.getDatablock().respawn * 1000, oreRespawn );
	}
}

function fxDTSBrick::oreRespawn( %this )
{
	if(isObject(%this))
	{
		%this.handleOreShape();
		%this.hasResources = true;
	}
}

package oreBrickPackage
{
	function fxDTSBrick::onRemove( %this )
	{
		if(isObject(%this.oreShape))
		{
			%this.oreShape.delete();
		}
		parent::onRemove( %this );
	}

	function fxDtsBrick::killBrick(%this)
	{
		if(isObject(%this.oreShape))
		{
			%this.oreShape.delete();
		}
		Parent::killBrick(%this);
	}

	function fxDTSBrick::onPlant( %this )
	{
		parent::onPlant( %this );
		%this.schedule(0, handleOreShape );
	}

	function fxDTSBrick::onLoadPlant( %this )
	{
		parent::onLoadPlant( %this );
		%this.schedule(0, handleOreShape );
	}

	function fxDTSBrick::handleOreShape(%this)
	{
		if(isObject(%this.oreShape))
		{
			%this.oreShape.delete();
		}
		if(%this.getDatablock() != nameToId("oreBrickData"))
		{
			return;
		}
		if(isObject(%this))
		{
			%this.resources = %this.getDatablock().resources;
			if(getWordCount(%this.getDatablock().ore) > 1)
			{
				%rnd = getRandom(0, getWordCount(%this.getDatablock().ore) - 1);
				echo(%rnd);
				%this.ore = getWord(%this.getDatablock().ore, %rnd);
				echo(getWord(%this.getDatablock().ore, %rnd));
			}

			if(%this.ore $= "iron")
				%color = "0.8 0.8 0.9 1";
			else if(%this.ore $= "gold")
				%color = "0.9 0.8 0 1";
			else if(%this.ore $= "copper")
				%color = "0.5 0.4 0.2 1";
			else
				%color = "1 0 1 0.5";

			%pos = getWords( %this.getPosition(), 0, 1) SPC getWord( %this.getPosition(), 2) - 0.9;
			%pos = %pos SPC %this.rotation;
			%this.oreShape = createOre(%pos, "1 1 1", %color);
		}
	}
};
activatePackage(oreBrickPackage);

function createOre(%pos, %scale, %color)
{
	%obj = new staticShape()
	{
		datablock = oreShapeData;
		scale = %scale;
		position = getWords(%pos, 0, 2);
		rotation = getWords(%pos, 3, 6);
	};
	missionCleanup.add( %obj );
	%obj.setNodeColor( "ALL", %color );
	echo(%obj.rotation);
	return %obj;
}

function IDtoRot(%id)
{
	switch(%id)
	{
		case 0:
			%trans = %trans SPC " 1 0 0 0";
		case 1:
			%trans = %trans SPC " 0 0 1" SPC $pi/2;
		case 2:
			%trans = %trans SPC " 0 0 1" SPC $pi;
		case 3:
			%trans = %trans SPC " 0 0 -1" SPC $pi/2;
	}
	return %trans;
}