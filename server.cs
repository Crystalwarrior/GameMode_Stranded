$BLS::Path = filePath(expandFileName("./description.txt")) @ "/";

function execBLS() {
	exec("./server.cs");
}

exec("./lib/misc.cs");
exec("./lib/blendRGBA.cs");
exec("./lib/AmmoSystem.cs");
exec("./lib/dayCycles.cs");
exec("./lib/decals.cs");
exec("./lib/hitbox.cs");
exec("./lib/raycastingWeapons.cs");

exec("./src/datablocks.cs");
exec("./src/core.cs");
exec("./src/package.cs");
exec("./src/footsteps.cs");
exec("./src/commands.cs");
exec("./src/damage.cs");
exec("./src/blood.cs");
exec("./src/examine.cs");
exec("./src/rpchat.cs");

exec("./src/bricks/ores/rocks.cs");
exec("./src/bricks/ores/trees.cs");

exec("./src/items/axe.cs");
exec("./src/items/pickaxe.cs");