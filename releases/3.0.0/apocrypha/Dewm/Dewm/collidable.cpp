#include "collidable.hpp"
#include "misc.hpp"

Collidable::~Collidable(){}

Collision earliestCollision(std::vector<Collidable*>& walls, Point2D pos, Vector2D vel, double radius)
{
  if (walls.size() == 0)
  {
    Collision result;
    result.t = NO_INTERSECTION;
    result.intersection = pos + vel;
    result.slide = Vector2D(0,0);
    return result;
  }

  std::vector<Collision> collisions;
  for (std::vector<Collidable*>::iterator iter = walls.begin(); iter < walls.end(); iter++)
  {
    collisions.push_back((*iter)->collide(pos, vel, radius));
  }
  std::sort(collisions.begin(), collisions.end());

  return collisions[0];
}

bool checkIsValidPosition(std::vector<Collidable*>& walls, Point2D pos, double r, Collidable* other)
{
  for (std::vector<Collidable*>::iterator iter = walls.begin(); iter < walls.end(); iter++)
  {
    if ((*iter)->isIntersecting(pos, r, other))
      return false;
  }
  return true;
}
