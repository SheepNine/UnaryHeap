# To do before release

* Review all of the code added to UnaryHeap libraries
** Double-check documentation XML for consistency/correctness
** Verify that new code added has good test coverage
* See what can be done to speed it up
** Burning a lot of time in sorting lists it seems
** How much is happening in GCD?
** Portalization is using a LOT of memory; see what can be optimized there
* Rename Qwtols something sensible, productize CLI interface so that it isn't so Godot-centric

* Heal T-Joins in BSP (3D only)
** Enhance test expectations with edge matching
*** Add Point3DComparer to determine if an edge is 'front' or 'back'
* Voronoi triangulation of surfaces