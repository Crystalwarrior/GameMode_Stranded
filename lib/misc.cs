function naturalGrammarList(%list) {
	%fields = getFieldCount(%list);

	if (%fields < 2) {
		return %list;
	}

	for (%i = 0; %i < %fields - 1; %i++) {
		%partial = %partial @ (%i ? ", " : "") @ getField(%list, %i);
	}

	return %partial SPC "and" SPC getField(%list, %fields - 1);
}

function vectorSpread(%vector, %spread) {
	%x = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
	%y = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
	%z = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;

	%mat = matrixCreateFromEuler(%x SPC %y SPC %z);
	return vectorNormalize(matrixMulVector(%mat, %vector));
}