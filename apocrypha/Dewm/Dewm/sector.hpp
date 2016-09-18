#ifndef _SECTOR_
#define _SECTOR_

#include "wall.hpp"
#include "line.hpp"
#include "texture.hpp"
#include "entity.hpp"
#include "particle.hpp"
#include "misc.hpp"
#include "projectile.hpp"
#include <vector>
#include <ostream>
#include <set>

class Sector{
public:
  Sector(std::vector<Wall*>& walls, WallTexture floorTex, int floorHeight, WallTexture ceilngTex, int ceilingHeight);
  void render2D();
  void render3D(Point2D pos, Point2D left, Point2D right, unsigned int bouncesLeft);
  Vector2D resolveCollisions(Point3D pos, Vector2D vel, double radius, double height, bool canStep, Collidable* other);
  void getCollidables(Point3D pos, Vector2D vel, double radius, double height, std::vector<Collidable*>& result, bool canStep, Collidable* other);
  Sector* getDestSector(Point2D start, Point3D& end);
  void nearestHeights(Point2D pos, double radius, int& floor, int& ceiling, Collidable* other);
  int floorHeight();
  int ceilingHeight();
  void addEntity(Entity* e);
  void removeEntity(Entity* e);
  void addParticle(ParticleBase* p);
  void addProjectile(ProjectileBase* p);
  void removeProjectile(ProjectileBase* p);
  void update(double time);
  HitscanResult resolveHitscan(Point3D pos, Vector3D trace, bool hitEntities, bool crossTeleportals, double& t, Point3D& dest, Sector* &camSector, void*& hit);
  Colour getFloorColour();
  Colour getCeilingColour();
  void getProjectiles(std::vector<ProjectileBase*>& result);
  bool hasSkyRoof();
  Vector3D findEntity(Point2D pos, Point2D left, Point2D right, Entity* target, unsigned int portalDepth);
  void getEntities(Point2D pos, double radius, std::vector<Entity*>& result);

  std::ostream& operator<<(std::ostream& out);

private:
  int F, C;
  WallTexture floor, ceiling;
  std::vector<Wall*> W;
  std::set<Entity*> entities;
  std::vector<ParticleBase*> P;
  std::vector<ProjectileBase*> projectiles;
};

#endif
