@REM Coloration flag

@Bin\MazeGenerator.exe "L:5" "H:n:X" "mc" "examples\color_off.svg"
@Bin\MazeGenerator.exe "L:5" "H:n:X" "mC" "examples\color_on.svg"


@REM Merge dead ends flag

@Bin\MazeGenerator.exe "L:10" "R:1234567" "mc" "examples\merge_off.svg"
@Bin\MazeGenerator.exe "L:10" "R:1234567" "Mc" "examples\merge_on.svg"


@REM Fortune maze layout highlighting

@Bin\MazeGenerator.exe "F:200:h:1234567" "H:n:Y" "mc" "examples\highlight_off.svg"
@Bin\MazeGenerator.exe "F:200:H:1234567" "H:n:Y" "mc" "examples\highlight_on.svg"
@Bin\MazeGenerator.exe "F:800:H:1234567" "H:n:Y" "mc" "examples\highlight_on_small.svg"


@REM Height maps

@Bin\MazeGenerator.exe "F:500:h:234567" "H:n:X"         "mc" "examples\heightmap_x.svg"
@Bin\MazeGenerator.exe "F:500:h:234567" "H:n:Y"         "mc" "examples\heightmap_y.svg"
@Bin\MazeGenerator.exe "F:500:h:234567" "H:n:X+Y"       "mc" "examples\heightmap_xysum.svg"
@Bin\MazeGenerator.exe "F:500:h:234567" "H:n:X-Y"       "mc" "examples\heightmap_xydiff.svg"
@Bin\MazeGenerator.exe "F:500:h:234567" "H:n:E:0,0"     "mc" "examples\heightmap_euclidorigin.svg"
@Bin\MazeGenerator.exe "F:500:h:234567" "H:n:E:250,375" "mc" "examples\heightmap_euclidoffset.svg"
@Bin\MazeGenerator.exe "F:500:h:234567" "H:n:M:0,0"     "mc" "examples\heightmap_manorigin.svg"
@Bin\MazeGenerator.exe "F:500:h:234567" "H:n:M:250,375" "mc" "examples\heightmap_manoffset.svg"
@Bin\MazeGenerator.exe "F:500:h:234567" "B"             "mc" "examples\biggestwall.svg"