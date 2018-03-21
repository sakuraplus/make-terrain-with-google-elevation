This is part of the Sinuous Sci-Fi Signs v1.5 package
Copyright (c) 2014-2018 Thomas Rasor
You can contact me through email : thomas.ir.rasor@gmail.com



The folder structure here ( .../src/Shaders ) is very important for shader compilation.
DO NOT move or remove folders or files here.
Your shaders WILL fail to compile if certain files are not where they are expected to be.


Required files for compilation, not included shaders themselves are:

-SSFSCG.cginc
-EVERYTHING in the "cginc" folder:
	-calc.cginc
	-maths.cginc
	-prototypes.cginc
	-sampling.cginc
	-structs.cginc
	-vars.cginc


If ANY of the previously listed files are missing or causing compilation errors, please contact me ASAP for help.
These are the files that form the core of the SSFS package, and they can be used to write SSFS variations.


SSFS.shader is the base shader file for all SSFS materials. DO NOT delete this file.
Even if you are writing your own version of it, it can be used as a reference.


SSFS_BasicHologram.shader is a shader completely separate from the SSFS base shader, useful when applied to arbitrary 3D models.
While materials using the two shaders might look similar, they share no code or references and SSFS_BasicHologram can be removed.