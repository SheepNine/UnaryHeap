#include "SDL_opengl.h"
#include "game.hpp"
#include "map.hpp"
#include "misc.hpp"
#include "algebra.hpp"

Game::Game(char* filename)
{
  map = new Map(filename);
  player = map->spawnEntities();

  pressedW = false;
  pressedA = false;
  pressedS = false;
  pressedD = false;
  pressedQ = false;
  pressedE = false;
  pressedR = false;
  pressedF = false;
  pressedShift = false;
  pressedLMouse = false;
  drawWireframed = false;
  drawMap = false;
  paintballMode = false;
  lastUpdateTimestamp = 0;
}

Game::~Game()
{
  delete map;
}

void Game::handleKeyboardEvent(SDL_KeyboardEvent evt)
{
  switch(evt.keysym.sym)
  {
  case(SDLK_w): pressedW = (evt.type == SDL_KEYDOWN); break;
  case(SDLK_a): pressedA = (evt.type == SDL_KEYDOWN); break;
  case(SDLK_s): pressedS = (evt.type == SDL_KEYDOWN); break;
  case(SDLK_d): pressedD = (evt.type == SDL_KEYDOWN); break;
  case(SDLK_q): pressedQ = (evt.type == SDL_KEYDOWN); break;
  case(SDLK_e): pressedE = (evt.type == SDL_KEYDOWN); break;
  case(SDLK_r): pressedR = (evt.type == SDL_KEYDOWN); break;
  case(SDLK_f): pressedF = (evt.type == SDL_KEYDOWN); break;
  case(SDLK_p): if (evt.type == SDL_KEYDOWN) paintballMode = !paintballMode; break;
  case(SDLK_LSHIFT): pressedShift = (evt.type == SDL_KEYDOWN); break;
  case(SDLK_1): if (evt.type == SDL_KEYDOWN) player->switchWeapon(0); break;
  case(SDLK_2): if (evt.type == SDL_KEYDOWN) player->switchWeapon(1); break;
  case(SDLK_3): if (evt.type == SDL_KEYDOWN) player->switchWeapon(2); break;
  case(SDLK_4): if (evt.type == SDL_KEYDOWN) player->switchWeapon(3); break;
  case(SDLK_m): if (evt.type == SDL_KEYDOWN) drawWireframed = !drawWireframed; break;
  case(SDLK_TAB): if (evt.type == SDL_KEYDOWN) drawMap = !drawMap; break;
  case(SDLK_SPACE): if (evt.type == SDL_KEYDOWN) player->jump(); break;
  }
}

void Game::handleMouseMotionEvent(SDL_MouseMotionEvent evt)
{
  player->rotate(evt.xrel / -5.0);
  player->look(evt.yrel / -5.0);
}

void Game::handleMouseButtonEvent(SDL_MouseButtonEvent evt)
{
  switch(evt.button)
  {
  case(SDL_BUTTON_LEFT): pressedLMouse = (evt.type == SDL_MOUSEBUTTONDOWN); break;
  case(SDL_BUTTON_WHEELUP): player->moveChaseCam(-1); break;
  case(SDL_BUTTON_WHEELDOWN): player->moveChaseCam(1); break;
  }
}

void Game::updateGamestate()
{
  unsigned int ticks = SDL_GetTicks();
  double delta = (ticks - lastUpdateTimestamp) / 1000.0;
  if (pressedShift) delta /= 20;

  if (pressedQ) player->rotate(9);
  if (pressedE) player->rotate(-9);
  if (pressedR) player->look(-9);
  if (pressedF) player->look(9);

  if (pressedW || pressedA || pressedS || pressedD)
  {
    Vector2D dir;
    if (pressedW) dir = dir + Vector2D(0, 1);
    if (pressedA) dir = dir + Vector2D(-1, 0);
    if (pressedS) dir = dir + Vector2D(0, -1);
    if (pressedD) dir = dir + Vector2D(1, 0);
    if (dir.length2() > 1) dir.normalize();
    player->move(dir);
  }
  if (pressedLMouse) player->fire(paintballMode);

  map->update(delta);
  lastUpdateTimestamp = ticks;
}

void Game::render()
{
  if (drawWireframed)
  {
    glDisable(GL_TEXTURE_2D);
    glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
  }
  else
  {
    glEnable(GL_TEXTURE_2D);
    glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
  }

  player->renderScene3D();
  if (drawMap)
    player->renderScene2D(map);
  else
    renderRetuicle();

  player->renderSBar();

  GLint error = glGetError();
  if (error != GL_NO_ERROR)
  {
    std::cout << gluErrorString(error) << std::endl;
  }
}

void Game::renderRetuicle()
{
  glDisable(GL_LIGHTING);
  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
  glMatrixMode(GL_PROJECTION);
  glLoadIdentity();
  GLint viewport[4];
  glGetIntegerv(GL_VIEWPORT, viewport);
  glOrtho(viewport[2] / -2.0, viewport[2] / 2.0, viewport[3] / -2.0, viewport[3] / 2.0, 0, 10000);
  glMatrixMode(GL_MODELVIEW);
  glLoadIdentity();

  glColor3d(1.0, 0.0, 0.0);
  glDepthFunc(GL_ALWAYS);
  glBegin(GL_LINES);
  {
    glVertex3d(-5, 0, 0);
    glVertex3d(6, 0, 0);

    glVertex3d(0, -5, 0);
    glVertex3d(0, 6, 0);
  }
  glEnd();
  glDepthFunc(GL_LEQUAL);

  glEnable(GL_LIGHTING);
}
