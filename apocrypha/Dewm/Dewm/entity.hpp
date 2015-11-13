#ifndef _ENTITY_
#define _ENTITY_

#include "algebra.hpp"
#include "collidable.hpp"
#include "drawable.hpp"

class Sector;

class Entity: public Collidable, public Drawable
{
public:
  Entity(Point2D pos, double a, Sector* s, double radius, double height);
  virtual void render2D() = 0;
  //virtual void render3D(int numReflections) = 0;
  virtual void render3D(Point2D pos, Point2D left, Point2D right) = 0;
  bool drawFirst() const;
  virtual void update(double time) = 0;
  virtual ~Entity();
  bool isVisible(Point2D pos, Point2D left, Point2D right);
  double resolveHitscan(Point3D pos, Vector3D trace);
  void damage(double amount);
  Collision collide(Point2D o, Vector2D v, double r);
  bool isIntersecting(Point2D p, double radius, Collidable* other);
  Point3D getPosition() const;
  bool isAlive();
protected:
  Sector* currSector;
  Point3D position;
  double angle, radius, height, health, cooldown;
  bool recentlyDamaged;
};

class Enemy : public Entity
{
public:
  Enemy(Point2D pos, double angle, Sector* s, Entity* target);
  ~Enemy();
  void render2D();
  virtual void render3D(Point2D pos, Point2D left, Point2D right);
  void update(double time);

private:
  Vector3D lastKnownView;
  Entity* player;
  double zVel;
};

#endif
