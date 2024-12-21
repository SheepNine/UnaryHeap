This repository contains the personal projects of Chris 'SheepNine' MacGregor.

The code contained herein is useful mostly to myself, but maybe you would be interested in doing the following:

* If you are doing work with graphs, the UnaryHeap.Graph library provides a data structure for a 2D graph, which can be passed into GraphRenderer to convert it into an SVG file.
* The Spatial library provides 2D and 3D implementations of game geometry pre-processing algorithms like constructive solid geometry, binary space partitioning, portal calculation and backface culling.
* Like mazes? MazeGenerator is a command-line app that can produce mazes in a JSON-based graph format. (The note above about GraphRenderer applies here as well.)
* Do you deploy software? Is it simple enough that a full Windows installer is overkill? PackageTool is a command-line tool that you can integrate into your MSBuild that will read an XML manifest and zip up your build artifacts.
* Is your hatred for long lines in .cs files total? LintRoller is a utility app that will report if your source code has long lines. (This codebase is all 98 characters/line or less, thanks to LintRoller.)
* Do you have a need for a homebrew version of Google Protocol buffers? Try Pocotheosis, for generating simple, serializable data objects from an easy-to-author manifest file.
* Would you like to play a game? There's a Reversi app.

The repository tries to adhere to the branching model described in [A successful Git branching model](http://nvie.com/posts/a-successful-git-branching-model/), with one notable exception: do not check out any 'cmac' branches. I use GitHub as a backup tool as well as a distribution platform, so these branches represent code that I wanted to save, but may not get released for a long time, if at all. I reserve the right to rebase, modify, or delete these at whim.

If you find a bug and want to fix it, I accept patches, but I would have to learn how to pull them first.