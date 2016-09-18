#ifndef _WALL_
#define _WALL_

#include "algebra.hpp"
#include "line.hpp"
#include <vector>
#include "texture.hpp"
#include <string>
#include "misc.hpp"

class Sector;

class Wall {
public:
  Wall(Point2D start, Point2D end, TextureSet tex, std::string flags, char ref);
  void setFrontSector(Sector* s);
  void render2D();
  void render3D(Point2D pos, Point2D left, Point2D right, unsigned int bouncesLeft);
  void link(Wall* other);
  Point2D getStartVertex();
  void getCollidables(Point3D pos, Vector2D vel, double radius, double height, std::vector<Collidable*>& result, bool canStep, Collidable* other);
  Sector* walkThrough(Point2D start, Point3D& end);
  void nearestHeights(Point2D pos, double radius, int& floor, int& ceiling, Collidable* other);
  bool isInside(Point2D pos);
  HitscanResult resolveHitscan(Point3D pos, Vector3D trace, bool hitEntities, bool crossTeleportals, double& t, Point3D& dest, Sector* &camSector, void*& hit);
  Vector3D findEntity(Point2D pos, Point2D left, Point2D right, Entity* target, unsigned int portalDepth);
  void getEntities(Point2D pos, double radius, std::vector<Entity*>& result);

  std::ostream& operator<<(std::ostream& out);
private:
  Line L;
  TextureSet T;
  Sector* F;
  Sector* B;
  char portalRef;
  bool isMirror, isTeleportal, isPortal;
  void drawStencil(int floor, int ceiling);
  void eraseStencil(int floor, int ceiling);
  void linkTeleportal(Wall* other);
  Matrix4x4 crossTransformation;
};

#endif
