#ifndef __GAME__
#define __GAME__

#include <vector>
#include "SDL.h"
#include "SDL_opengl.h"
#include "map.hpp"
#include "algebra.hpp"
#include "player.hpp"

class Game
{
public:
  Game(char* filename);
  virtual ~Game();
  void handleKeyboardEvent(SDL_KeyboardEvent evt);
  void handleMouseMotionEvent(SDL_MouseMotionEvent evt);
  void handleMouseButtonEvent(SDL_MouseButtonEvent evt);
  void updateGamestate();
  void render();

private:
  void renderWireframeMap();
  void toggleWireframe();
  void renderRetuicle();

  Map* map;
  Player* player;
  bool pressedW, pressedA, pressedS, pressedD, pressedQ, pressedE, pressedR, pressedF, pressedLMouse, pressedShift;
  bool drawWireframed, drawMap, paintballMode;
  unsigned int lastUpdateTimestamp;
};

#endif
