$FS::CheckInterval = 50;
$FS::DefaultMaterial = "dirt";

package footstepPackage
{
	function gameConnection::spawnPlayer( %this )
	{
		parent::spawnPlayer( %this );
		%this.player.fs_velocityCheckTick();
	}

	function armor::onTrigger( %this, %obj, %slot, %val )
	{
		parent::onTrigger( %this, %obj, %slot, %val );

		if ( %slot == 4 && !%this.canJet )
		{
			%val = false;
		}

		%obj.fs_trigger[ %slot ] = %val;
	}
};

activatePackage( "footstepPackage" );

function getNumberStart( %str )
{
	%best = -1;

	for ( %i = 0 ; %i < 10 ; %i++ )
	{
		%pos = strPos( %str, %i );

		if ( %pos < 0 )
		{
			continue;
		}

		if ( %best == -1 || %pos < %best )
		{
			%best = %pos;
		}
	}

	return %best;
}

function loadFootstepSounds()
{
	%pattern = $BLS::Path @ "res/sounds/footsteps/*.wav";
	%list = "generic 0";

	deleteVariables( "$FS::Sound*" );
	$FS::SoundNum = 0;

	for ( %file = findFirstFile( %pattern ) ; %file !$= "" ; %file = findNextFile( %pattern ) )
	{
		%base = fileBase( %file );
		%name = "footstepSound_" @ %base;

		if ( !isObject( %name ) )
		{
			datablock audioProfile( genericFootstepSound )
			{
				description = "audioClosest3D";
				fileName = %file;
				preload = true;
			};

			if ( !isObject( %obj = nameToID( "genericFootstepSound" ) ) )
			{
				continue;
			}

			%obj.setName( %name );
		}

		if ( ( %pos = getNumberStart( %base ) ) > 0 )
		{
			%pre = getSubStr( %base, 0, %pos );
			%post = getSubStr( %base, %pos, strLen( %base ) );

			if ( $FS::SoundCount[ %pre ] < 1 || !strLen( $FS::SoundCount[ %pre ] ) )
			{
				%list = %list SPC %pre SPC $FS::SoundNum;
			}

			if ( $FS::SoundCount[ %pre ] < %post )
			{
				$FS::SoundCount[ %pre ] = %post;
			}

			$FS::SoundName[ $FS::SoundNum ] = %pre;
			$FS::SoundIndex[ %pre ] = $FS::SoundNum;
			$FS::SoundNum++;
		}
	}

	registerOutputEvent( "fxDTSBrick", "setMaterial", "list" SPC %list );
}

function fxDTSBrick::setMaterial( %this, %idx )
{
	%this.material = $FS::SoundName[ %idx ];
}

function playFootstep( %pos, %material )
{
	if ( !strLen( %material ) || $FS::SoundCount[ %material ] < 1 )
	{
		return;
	}

	if ( !isObject( %sound = nameToID( "footstepSound_" @ %material @ getRandom( 1, $FS::SoundCount[ %material ] ) ) ) )
	{
		return;
	}

	serverPlay3D( %sound, %pos );
}
//
//function Player::getFootMaterial( %this )
//{
//	%pos = vectorAdd( %this.getPosition(), "0 0 1" );
//	%feetPos = vectorSub( %pos, "0 0 1.1" );
//	%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::ShapeBaseObjectType;
//	return determineObjectMaterial( firstWord( containerRayCast( %pos, %feetPos, %mask, %this ) ) );
//}

function player::getFootMaterial( %this )
{
	if (mAbs(getWord(%this.getVelocity(), 2)) < 0.01) {
		return $FS::DefaultMaterial;
	}

	%offset[ 0 ] = "0 0";
	%offset[ 1 ] = "0.5 0";
	%offset[ 2 ] = "-0.5 0";
	%offset[ 3 ] = "0 0.5";
	%offset[ 4 ] = "0 -0.5";

	for ( %i = 0 ; %i < 5 ; %i++ )
	{
		if ( isObject( %this._isOnGround( %offset[ %i ] ) ) )
		{
			return determineObjectMaterial( %this._isOnGround( %offset[ %i ] ) );
		}
	}
	return false;
}

function player::_isOnGround( %this, %offset )
{
	%start = vectorAdd( %this.position, %offset SPC "0.4" );
	%stop = vectorSub( %this.position, %offset SPC "0.4" );
	%raycast = containerRayCast( %start, %stop, $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType, %this );

	return firstWord( %raycast );
}

function Player::getFootLocation( %this )
{
	return vectorSub( %this.getPosition(), "0 0 0.1" );
}

function Player::fs_velocityCheckTick( %this )
{
	cancel( %this.fs_velocityCheckTick );

	%vz = getWord(%this.getVelocity(), 2);

	if (%this.lastVZ < -6 && %vz >= -0.75) {
		%this.playJumpLanding();
	}

	%this.lastVZ = %vz;

	if ( vectorLen( %this.getVelocity() ) >= 2.62 && !isEventPending( %this.fs_playTick ) )
	{
		%this.fs_playTick = %this.schedule( 100, "fs_playTick" );
	}

	%this.fs_velocityCheckTick = %this.schedule( $FS::CheckInterval, "fs_velocityCheckTick" );
}

function player::fs_playTick( %this )
{
	cancel( %this.fs_playTick );

	if ( vectorLen( %this.getVelocity() ) < %this.getDatablock().maxForwardSpeed / 2.67 )
	{
		return;
	}

	if ( %this.fs_trigger[ 3 ] || %this.fs_trigger[ 4 ] || %this.dead )
	{
		return;
	}

	%material = %this.getFootMaterial();

	if ( %material $= "" )
	{
		return;
	}

	playFootstep( %this.getFootLocation(), %material );
	%this.fs_playTick = %this.schedule( 290, "fs_playTick" );
}

function determineObjectMaterial( %obj )
{
	if ( !isObject( %obj ) )
	{
		return "";
	}

	if ( strLen( %obj.material ) )
	{
		return %obj.material;
	}

	%class = %obj.getClassName();

	if ( isFunction( %class, "getType" ) )
	{
		%type = %obj.getType();
	}

	if ( %class $= "fxDTSBrick" )
	{
		%color = %obj.colorID;
	}

	return $FS::DefaultMaterial;
}

function Player::playJumpLanding(%this) {
	serverPlay3D(jumpLandingSound, %this.getPosition());
}

loadFootstepSounds();