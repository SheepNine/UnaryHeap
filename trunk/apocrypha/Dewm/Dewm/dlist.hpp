#ifndef _DLIST_
#define _DLIST_

#include "SDL_opengl.h"

#define NUM_DISPLAY_LISTS 3
#define SKYBOX_LIST 0
#define MISSILE_LIST 1
#define FIREBALL_LIST 2
extern GLuint displayListHandles[NUM_DISPLAY_LISTS];

void createLists();

#endif
