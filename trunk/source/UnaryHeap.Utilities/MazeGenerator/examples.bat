@REM Coloration flag

@MazeGenerator.exe "L:5" "H:n:X" "mc" "examples\color_off.svg"
@MazeGenerator.exe "L:5" "H:n:X" "mC" "examples\color_on.svg"

@REM Merge dead ends flag

@MazeGenerator.exe "L:10" "R:1234567" "mc" "examples\merge_off.svg"
@MazeGenerator.exe "L:10" "R:1234567" "Mc" "examples\merge_on.svg"

@REM Fortune maze layout highlighting

@MazeGenerator.exe "F:200:h:1234567" "H:n:Y" "mc" "examples\highlight_off.svg"
@MazeGenerator.exe "F:200:H:1234567" "H:n:Y" "mc" "examples\highlight_on.svg"
@MazeGenerator.exe "F:800:H:1234567" "H:n:Y" "mc" "examples\highlight_on_small.svg"

@REM Height maps

@MazeGenerator.exe "F:500:h:234567" "H:n:X"         "mc" "examples\heightmap_x.svg"
@MazeGenerator.exe "F:500:h:234567" "H:n:Y"         "mc" "examples\heightmap_y.svg"
@MazeGenerator.exe "F:500:h:234567" "H:n:X+Y"       "mc" "examples\heightmap_xysum.svg"
@MazeGenerator.exe "F:500:h:234567" "H:n:X-Y"       "mc" "examples\heightmap_xydiff.svg"
@MazeGenerator.exe "F:500:h:234567" "H:n:E:0,0"     "mc" "examples\heightmap_euclidorigin.svg"
@MazeGenerator.exe "F:500:h:234567" "H:n:E:250,375" "mc" "examples\heightmap_euclidoffset.svg"
@MazeGenerator.exe "F:500:h:234567" "H:n:M:0,0"     "mc" "examples\heightmap_manorigin.svg"
@MazeGenerator.exe "F:500:h:234567" "H:n:M:250,375" "mc" "examples\heightmap_manoffset.svg"
@MazeGenerator.exe "F:500:h:234567" "B"             "mc" "examples\biggestwall.svg"