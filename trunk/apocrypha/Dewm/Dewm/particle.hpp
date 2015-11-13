#ifndef _PARTICLE_
#define _PARTICLE_

#include "algebra.hpp"
#include "drawable.hpp"

class ParticleBase: public Drawable
{
public:
  ParticleBase(Point3D p, Vector3D v, double t, bool g);
  virtual ~ParticleBase();
  virtual void render3D(Point2D pos, Point2D left, Point2D right) = 0;
  void update(double time);
  bool isDead();
  Point3D getPosition() const;
  virtual bool drawFirst() const = 0;
protected:
  Point3D position;
  Vector3D velocity;
  double timeLeft;
  bool isGravitied;
};

class QuakeParticle : public ParticleBase
{
public:
  QuakeParticle(Point3D p, Vector3D v, double t, Colour c);
  QuakeParticle(Point3D p, Vector3D v, double t, Colour c, bool gravity);
  virtual ~QuakeParticle();
  void render3D(Point2D pos, Point2D left, Point2D right);
  virtual bool drawFirst() const;
private:
  Colour colour;
  double initialTime;
};

class TraceParticle: public ParticleBase
{
public:
  TraceParticle(Point3D p, Vector3D v, double t, Colour c);
  virtual ~TraceParticle();
  void render3D(Point2D pos, Point2D left, Point2D right);
  virtual bool drawFirst() const;
private:
  Colour colour;
  double initialTime;
};

class RicochetParticle: public ParticleBase
{
public:
  RicochetParticle(Point3D p, double t);
  virtual ~RicochetParticle();
  void render3D(Point2D pos, Point2D left, Point2D right);
  virtual bool drawFirst() const;
private:
  double initialTime;
};

class BoomParticle : public ParticleBase
{
public:
  BoomParticle(Point3D p, double t, double ms, Colour s, Colour e);
  virtual ~BoomParticle();
  void render3D(Point2D pos, Point2D left, Point2D right);
  virtual bool drawFirst() const;
private:
  double initialTime;
  double maxSize;
  Colour start, end;
};

class BulletHole : public ParticleBase
{
public:
  enum Surface { FLOOR, CEILING, WALL };
  BulletHole(Surface s, Point3D p, Vector3D n, double t, double big, Colour color);
  virtual ~BulletHole();
  void render3D(Point2D pos, Point2D left, Point2D right);
  virtual bool drawFirst() const;
private:
  Vector3D normal;
  Surface surface;
  double size;
  Colour color;
};

#endif
