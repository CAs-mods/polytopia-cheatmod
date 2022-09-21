REFS := $(wildcard libs/*.dll)

CheatMod.dll: CheatMod.cs
	echo $(REFS)
	mcs CheatMod.cs -target:library -out:CheatMod.dll $(addprefix -reference:,$(REFS))
