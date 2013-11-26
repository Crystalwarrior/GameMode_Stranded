datablock ItemData( blsLogItem : swordItem )
{
	shapeFile = $BLS::Path @ "res/Shapes/log.dts";
	mass = 1.2;
	density = 0.1;
	elasticity = 0.1;
	friction = 0.8;
	emap = true;

	uiName = "Log";
	iconName = "";
	doColorShift = true;
	colorShiftColor = "0.54 0.27 0.07 1";

	image = "";
	canDrop = true;

	//BLS Inventory Stuff
	invDesc = "It's a log. Can be used for building bricks resembling logs."; //idk
	invMass = 50;
	stack = 1;
};

datablock itemData( blsLeafItem : blsLogItem )
{
	shapeFile = $BLS::Path @ "res/Shapes/leaf.dts";
	mass = 0.6;
	friction = 0.4;
	density = 0.5;

	uiName = "Leaf";
	doColorShift = false;

	//BLS Inventory Stuff
	invDesc = "Leaves! They're so useful.";
	invMass = 0.01;
	stack = 99;
};

datablock itemData( blsCopperOreItem : blsLogItem )
{
	shapeFile = $BLS::Path @ "res/Shapes/ore.dts";
	mass = 1;
	friction = 0.6;
	density = 0.1;

	uiName = "Copper Ore";
	doColorShift = true;
	colorShiftColor = "0.5 0.4 0.2 1";

	//BLS Inventory Stuff
	invDesc = "Can be smelted into copper bars.";
	invMass = 6.2;
	stack = 16;
};

datablock itemData( blsGoldOreItem : blsCopperOreItem )
{
	mass = 1;
	friction = 0.6;

	uiName = "Gold Ore";
	doColorShift = true;
	colorShiftColor = "0.9 0.8 0 1";

	//BLS Inventory Stuff
	invDesc = "I'm rich! Can be smelted into golden bars.";
	invMass = 4.1;
	stack = 16;
};

datablock itemData( blsIronOreItem : blsCopperOreItem )
{
	mass = 1;
	friction = 0.6;

	uiName = "Iron Ore";
	doColorShift = true;
	colorShiftColor = "0.8 0.8 0.9 1";

	//BLS Inventory Stuff
	invDesc = "Something sturdy. Can be smelted into iron bars.";
	invMass = 8.3;
	stack = 16;
};

datablock itemData( blsStoneItem : blsCopperOreItem )
{
	mass = 1;
	friction = 0.6;

	uiName = "Stone";
	doColorShift = true;
	colorShiftColor = "0.657 0.657 0.657 1";

	//BLS Inventory Stuff
	invDesc = "A stone. You can throw it.";
	invMass = 6;
	stack = 16;
};

datablock ItemData( blsPlankItem : blsLogItem )
{
	shapeFile = $BLS::Path @ "res/Shapes/plank.dts";
	mass = 1;
	density = 0.2;
	friction = 0.7;

	uiName = "Plank";
	doColorShift = true;
	colorShiftColor = "0.74 0.37 0.17 1";

	//BLS Inventory Stuff
	invDesc = "It's a plank! Can be used to build stuff."; //lol kk
	invMass = 1;
	stack = 32;
};

datablock ItemData( blsBranchItem : blsLogItem )
{
	shapeFile = $BLS::Path @ "res/Shapes/branch.dts";
	mass = 1;
	density = 0.2;
	friction = 0.7;

	uiName = "Branch";
	doColorShift = true;
	colorShiftColor = "0.54 0.27 0.07 1";

	//BLS Inventory Stuff
	invDesc = "It's a branch. Poke stuff with it or something.";
	invMass = 0.5;
	stack = 64;
};

datablock ItemData( blsIronBarItem : blsLogItem )
{
	shapeFile = $BLS::Path @ "res/Shapes/Bar.dts";
	mass = 1;
	density = 0.1;
	friction = 0.7;

	uiName = "Iron Bar";
	doColorShift = true;
	colorShiftColor = "0.82 0.82 0.92 1";

	//BLS Inventory Stuff
	invDesc = "Make sturdy stuff with it!";
	invMass = 5;
	stack = 32;
};
