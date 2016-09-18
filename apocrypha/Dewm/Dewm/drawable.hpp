#ifndef _DRAWABLE_
#define _DRAWABLE_

#include "algebra.hpp"

class Drawable
{
public:
  virtual ~Drawable();
  virtual Point3D getPosition() const = 0;
  virtual void render3D(Point2D pos, Point2D left, Point2D right) = 0;
  virtual bool drawFirst() const = 0;
};

class CompareDrawableDepth
{
public:
  CompareDrawableDepth(Point2D pos);
  bool operator() (const Drawable* b1, const Drawable* b2) const;
private:
  Point2D refPos;
};

#endif
