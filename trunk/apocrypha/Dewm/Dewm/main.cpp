#include "SDL.h"
#include "SDL_opengl.h"
#include <iostream>
#include <time.h>
#include "game.hpp"
#include "texture.hpp"
#include "dlist.hpp"

#define UPDATE_GAMESTATE_EVENT_CODE 1
#define MS_PER_FRAME 25

Game* game;

//============================================================================
// Initialization methods
//============================================================================

void resize(GLsizei width, GLsizei height)
{
  glViewport(0,0,width,height);

  glMatrixMode(GL_PROJECTION);
  glLoadIdentity();
  gluPerspective(40.0, (GLfloat)width/(GLfloat)height, 0.1, 1000.0);

  glMatrixMode(GL_MODELVIEW);
  glLoadIdentity();
}

void initGL()
{
  glEnable(GL_NORMALIZE);
  glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
  glEnable(GL_DEPTH_TEST);
  glDepthFunc(GL_LEQUAL);
  glEnable(GL_POINT_SMOOTH);
  glEnable(GL_CULL_FACE);  
  glCullFace(GL_BACK);

  // Set up all that lighting model stuff
  glEnable(GL_LIGHTING);
  glEnable(GL_COLOR_MATERIAL);
  glColorMaterial(GL_FRONT_AND_BACK, GL_AMBIENT_AND_DIFFUSE);
  glLightModelf(GL_LIGHT_MODEL_LOCAL_VIEWER, GL_TRUE);

  GLfloat specMat[4] = {0.0f, 0.0f, 0.0f, 0.0f};
  glMaterialfv(GL_FRONT_AND_BACK, GL_SPECULAR, specMat);

  GLfloat ambG[4] = {0.0f, 0.0f, 0.0f, 0.0f};
  glLightModelfv(GL_LIGHT_MODEL_AMBIENT, ambG);

  glEnable(GL_LIGHT0);
  GLfloat amb0[4] = {0.4f, 0.4f, 0.4f, 1.0f};
  glLightfv(GL_LIGHT0, GL_AMBIENT, amb0);
  GLfloat diff0[4] = {0.2f, 0.2f, 0.2f, 1.0f};
  glLightfv(GL_LIGHT0, GL_DIFFUSE, diff0);
  GLfloat spec0[4] = {0.0f, 0.0f, 0.0f, 0.0f};
  glLightfv(GL_LIGHT0, GL_SPECULAR, spec0);

  // Set up texturing parameters

  glEnable(GL_TEXTURE_2D);
  createTextures();

  createLists();
  // Set up blending
  glEnable(GL_BLEND);
  glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

  // Set up depth test
  glEnable(GL_STENCIL_TEST);
  glStencilFunc(GL_EQUAL, 0, 0xFFFF);
  glStencilOp(GL_KEEP, GL_KEEP, GL_KEEP);

  // Draw to the back buffer only
  glDrawBuffer(GL_BACK);
}

int createGLWindow(char* title, int width, int height, bool fullscreen)
{
	if ( SDL_Init(SDL_INIT_VIDEO | SDL_INIT_TIMER) < 0 )
  {
    std::cerr << "createGLWindow failed: Couldn't init SDL: " << SDL_GetError() << std::endl;
		return 0;
	}

  if (SDL_GL_SetAttribute( SDL_GL_DOUBLEBUFFER, 1 ) < 0)
  {
    std::cerr << "Couldn't set double buffer value" << std::endl;
    return 0;

  }
  if (SDL_GL_SetAttribute( SDL_GL_STENCIL_SIZE, 8) < 0)
  {
    std::cerr << "Couldn't set stencil depth" << std::endl;
    return 0;
  }

	Uint32 flags = SDL_OPENGL | SDL_DOUBLEBUF;
	if ( fullscreen )
  {
		flags |= SDL_FULLSCREEN;
		width = 0;
		height = 0;
	}
  SDL_WM_SetCaption(title, NULL);
	SDL_Surface* surf = SDL_SetVideoMode(width, height, 0, flags);
	if ( surf == NULL )
  {
    std::cerr << "createGLWindow failed: Couldn't init surface" << std::endl;
   return 0;
  }

  int stencilBitplanes;
  if (SDL_GL_GetAttribute(SDL_GL_STENCIL_SIZE, &stencilBitplanes) != 0) std::cerr << "Stencil read error" << std::endl;
  std::cout << "Stencil buffer initialized to " << stencilBitplanes << " bits" << std::endl;

	resize(surf->w, surf->h);

  initGL();

	return 1;
}

//============================================================================
// Event handlers
//============================================================================

void redraw()
{
  //unsigned int time = SDL_GetTicks();
  glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);

  game->render();
  glFinish();

  SDL_GL_SwapBuffers();
  /*unsigned int delta = SDL_GetTicks() - time;
  if (delta > MS_PER_FRAME) std::cout << "**";
  std::cout << (delta / 1000.0) << "ms, ";*/
}

bool handleResizeEvent(SDL_ResizeEvent evt)
{
  resize(evt.w, evt.h);
  return true;
}

bool handleKeyboardEvent(SDL_KeyboardEvent evt)
{
  game->handleKeyboardEvent(evt);
  return false;
}

bool handleMouseMotionEvent(SDL_MouseMotionEvent evt)
{
  game->handleMouseMotionEvent(evt);
  return false;
}

bool handleMouseButtonEvent(SDL_MouseButtonEvent evt)
{
  game->handleMouseButtonEvent(evt);
  return false;
}

bool handleUserEvent(SDL_UserEvent evt)
{
  if (evt.code == UPDATE_GAMESTATE_EVENT_CODE) game->updateGamestate();
  return true;
}

//============================================================================
// Event generators
//============================================================================

Uint32 updatePhysicsTimer(Uint32 interval, void* params)
{
  SDL_Event updateEvent;
  updateEvent.type = SDL_USEREVENT;
  updateEvent.user.code = UPDATE_GAMESTATE_EVENT_CODE;
  updateEvent.user.data1 = NULL;
  updateEvent.user.data2 = NULL;
  SDL_PushEvent(&updateEvent);

  return interval;
}

//============================================================================
// Main loop
//============================================================================

int main(int argc, char *argv[])
{
  bool fullscreen;

  if (argc < 3)
  {
    std::cerr << "usage: " << argv[0] << " <mapfile> <fullscreen?> [usestencil?]" << std::endl
      << "fullscreen/usestencil should be either T or F" << std::endl;
    return(5);
  }
  fullscreen = strcmp(argv[2], "T") == 0;
  if (argc > 3)
    STENCIL_ENABLED = strcmp(argv[3], "T") == 0;
  else
    STENCIL_ENABLED = true;

  game = new Game(argv[1]);

  srand((unsigned int)time(NULL));
  Uint8* keys;

  if(createGLWindow("DewM", 640, 480, fullscreen) == 0)
  {
    SDL_Quit();
    return 1;
  }

  // Check for a stencil buffer
  GLint stencilSize;
  glGetIntegerv(GL_STENCIL_BITS, &stencilSize);
  if (stencilSize == 0 && STENCIL_ENABLED)
  {
    std::cerr << "NO STENCIL BUFFER DETECTED!" << std::endl
      << "To run the program, set [usestencil] to F" << std::endl;
    SDL_Quit();
    delete game;
    return 0;
  }

  SDL_ShowCursor(0);
  if (fullscreen)
    SDL_WM_GrabInput(SDL_GRAB_FULLSCREEN);
  else
    SDL_WM_GrabInput(SDL_GRAB_ON);


  SDL_AddTimer(MS_PER_FRAME, updatePhysicsTimer, NULL);

  bool done = false;
  bool redrawNeeded = false;

  while(!done)
  {
    // Redraw if necessary
    if (redrawNeeded)
    {
      redraw();
      redrawNeeded = false;
    }

    // And process events
    SDL_Event event;
    while (SDL_PollEvent(&event))
    {
      switch (event.type)
      {
        case SDL_KEYDOWN:
        case SDL_KEYUP:
          redrawNeeded |= handleKeyboardEvent(event.key);
        break;

        case SDL_MOUSEMOTION:
          redrawNeeded |= handleMouseMotionEvent(event.motion);
        break;

        case SDL_MOUSEBUTTONDOWN:
        case SDL_MOUSEBUTTONUP:
          redrawNeeded |= handleMouseButtonEvent(event.button);
        break;

        case SDL_USEREVENT:
          redrawNeeded |= handleUserEvent(event.user);
        break;

        case SDL_VIDEORESIZE:
          redrawNeeded |= handleResizeEvent(event.resize);
        break;

        case SDL_QUIT:
          done=1;
        break;

        default:
        break;
      }
    }
    
    keys = SDL_GetKeyState(NULL);
    if(keys[SDLK_ESCAPE]) done=1;
  }
  
  SDL_Quit();
  delete game;
  return 0;
}

// ----------- VS 2015 compatability hack ------------

FILE _iob[] = { *stdin, *stdout, *stderr };

extern "C" FILE * __cdecl __iob_func(void)
{
	return _iob;
}