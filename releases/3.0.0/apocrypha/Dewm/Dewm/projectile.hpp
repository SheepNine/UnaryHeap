#ifndef _PROJECTILE_
#define _PROJECTILE_

#include "algebra.hpp"
#include "entity.hpp"
#include "line.hpp"
#include "drawable.hpp"

class Sector;

class ProjectileBase: public Drawable
{
public:
  ProjectileBase(Point3D pos, Vector3D vel, Sector* cs, bool useGrav);
  virtual ~ProjectileBase();
  virtual void render3D(Point2D pos, Point2D left, Point2D right) = 0;
  void update(double time);
  virtual void wallCollide(Point3D pos, Line* wall) = 0;
  virtual void floorCollide(Point3D pos) = 0;
  virtual void ceilingCollide(Point3D pos) = 0;
  virtual void entityCollide(Point3D pos, Entity* victim) = 0;
  virtual void onUpdate(double time) = 0;
  Point3D getPosition() const;
  virtual bool drawFirst() const;
  bool isDead();
protected:
  Point3D position;
  Vector3D velocity;
  Sector* currSector;
private:
  bool isGravitated, hasHit;
};

class Rocket: public ProjectileBase
{
public:
  Rocket(Point3D pos, Vector3D vel, Sector* cs);
  virtual ~Rocket();
  void render3D(Point2D pos, Point2D left, Point2D right);
  void wallCollide(Point3D pos, Line* wall);
  void floorCollide(Point3D pos);
  void ceilingCollide(Point3D pos);
  void entityCollide(Point3D pos, Entity* victim);
  void onUpdate(double delta);
private:
  void explode();
  double smokeTime;
};

class Fireball: public ProjectileBase
{
public:
  Fireball(Point3D pos, Vector3D vel, Sector* cs);
  virtual ~Fireball();
  void render3D(Point2D pos, Point2D left, Point2D right);
  void wallCollide(Point3D pos, Line* wall);
  void floorCollide(Point3D pos);
  void ceilingCollide(Point3D pos);
  void entityCollide(Point3D pos, Entity* victim);
  void onUpdate(double delta);
private:
  double smokeTime;
  double age;
};

#endif
