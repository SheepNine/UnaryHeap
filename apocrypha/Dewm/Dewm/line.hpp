#ifndef _LINE_
#define _LINE_

#include "algebra.hpp"
#include <vector>
#include <ostream>
#include "collidable.hpp"

std::ostream& operator<< (std::ostream& out, Collision& c);

bool operator<(Collision a, Collision b);

class Collidable;

class Line: public Collidable {
public:
  Line();
  virtual ~Line();
  Line(Point2D start, Point2D end);
  void render2D();
  void render3D(int floor, int ceiling, double offsetX, double offsetY);
  Collision collide(Point2D o, Vector2D v, double r);
  //bool isntTouching(Point2D pos, double r);
  bool isIntersecting(Point2D p, double r, Collidable* other);
  bool isInFront(Point2D pos);
  bool isInFront(Point2D pos, double radius);
  bool isBackfaceOf(Line other);
  Point2D getStartVertex();
  Point2D getEndVertex();
  void reflectMVMatrix(Point2D orig);
  Point2D reflectPoint(Point2D point);
  bool clip(Point2D pos, Point2D left, Point2D right);
  bool clip(Line clippingPlane);
  bool clip(std::vector<Point2D>& vertices);
  Vector3D getRicochetVector();
  Matrix4x4 getTransformation(Line& other);
  Vector3D getNormal();

  std::ostream& operator<<(std::ostream& out);
private:
  Point2D S;
  Point2D E;
  Vector2D N;
};

#endif
