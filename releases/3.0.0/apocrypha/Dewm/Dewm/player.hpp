#ifndef _PLAYER_
#define _PLAYER_

#include "algebra.hpp"
#include "sector.hpp"
#include "entity.hpp"

class Map;

class Player: public Entity {
public:
  Player(Point2D pos, double a, Sector* s);
  ~Player();
  void render2D();
  void render3D(Point2D pos, Point2D left, Point2D right);
  void render3DForCamera();
  void update(double time);

  void renderScene2D(Map* map);
  void renderScene3D();
  void renderSBar();

  void switchWeapon(int weapon);
  void move(Vector2D dir);
  void rotate(double angle);
  void look(double angle);
  void fire(bool paintballMode);
  void jump();
  void moveChaseCam(double delta);
private:
  double cameraAngle;
  double speed;
  double lookangle;
  int weapon;
  Vector2D dir;
  double zVel;
  double displayHealth;
  double cameraDist, desiredCameraDist;
};

#endif
