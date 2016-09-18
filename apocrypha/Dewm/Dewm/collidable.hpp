#ifndef _COLLIDABLE_
#define _COLLIDABLE_

#include <vector>
#include "algebra.hpp"

struct Collision {
  double t;
  Point2D intersection;
  Vector2D slide;
};

class Collidable
{
public:
  virtual ~Collidable();
  virtual Collision collide(Point2D o, Vector2D v, double r) = 0;
  virtual bool isIntersecting(Point2D p, double r, Collidable* other) = 0;
};

Collision earliestCollision(std::vector<Collidable*>& walls, Point2D pos, Vector2D vel, double radius);
bool checkIsValidPosition(std::vector<Collidable*>& walls, Point2D pos, double r, Collidable* other);

#endif
