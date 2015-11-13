#include "line.hpp"
#include "misc.hpp"

std::ostream& operator<< (std::ostream& out, Collision& c)
{
  return out << c.t << " to " << c.intersection << " slide " << c.slide;
}

Line::Line()
{
  S = Point2D(0,0);
  E = Point2D(1,0);
  N = Vector2D(0,1);
}

Line::Line(Point2D start, Point2D end): S(start), E(end)
{
  N = calcNormal(S, E);
}

Line::~Line() {}

void Line::render2D()
{
  glBegin(GL_LINES);
  {
    glVertex3d(S[0], S[1], 0);
    glVertex3d(E[0], E[1], 0);

    Point2D midpoint (0.5 * (S[0] + E[0]), 0.5 * (S[1] + E[1]));
    Point2D normalEnd = midpoint + N;

    glVertex3d(midpoint[0], midpoint[1], 0);
    glVertex3d(normalEnd[0], normalEnd[1], 0);
  }
  glEnd();
}

void Line::render3D(int floor, int ceiling, double offsetX, double offsetY)
{
  glNormal3f((GLfloat)N[0], (GLfloat)N[1], 0.0f);
  double cs = (S-E).length() / 8.0;
  glBegin(GL_TRIANGLES);
  {
#ifdef SPLIT_WALLS_INTO_LEVELS
    for (int layer = floor; layer < ceiling; layer++)
    {
      double ct = (layer - floor) / 8.0;

      glTexCoord2d(cs + offsetX, ct + 0.125 + offsetY);
      glVertex3d(S[0], S[1], layer + 1);
      glTexCoord2d(0 + offsetX, ct + 0.125 + offsetY);
      glVertex3d(E[0], E[1], layer + 1);
      glTexCoord2d(cs + offsetX, ct + offsetY);
      glVertex3d(S[0], S[1], layer);

      glTexCoord2d(cs + offsetX, ct + offsetY);
      glVertex3d(S[0], S[1], layer);
      glTexCoord2d(0 + offsetX, ct + 0.125 + offsetY);
      glVertex3d(E[0], E[1], layer + 1);
      glTexCoord2d(0 + offsetX, ct + offsetY);
      glVertex3d(E[0], E[1], layer);
    }
#else
    double ct = (ceiling - floor) / 8.0;
    glTexCoord2d(cs + offsetX, ct + offsetY);
    glVertex3d(S[0], S[1], ceiling);
    glTexCoord2d(0 + offsetX, ct + offsetY);
    glVertex3d(E[0], E[1], ceiling);
    glTexCoord2d(cs + offsetX, 0 + offsetY);
    glVertex3d(S[0], S[1], floor);

    glTexCoord2d(cs + offsetX, 0 + offsetY);
    glVertex3d(S[0], S[1], floor);
    glTexCoord2d(0 + offsetX, ct + offsetY);
    glVertex3d(E[0], E[1], ceiling);
    glTexCoord2d(0 + offsetX, 0 + offsetY);
    glVertex3d(E[0], E[1], floor);
#endif
  }
  glEnd();
}

Collision Line::collide(Point2D O, Vector2D V, double R)
{
  double lineT = intersectLineSegment(O, V, S + R * N, E + R * N);
  double startT = intersectCircle(O, V, S, R);
  double endT = intersectCircle(O, V, E, R);

  Collision result;
  if (lineT == NO_INTERSECTION && startT == NO_INTERSECTION && endT == NO_INTERSECTION)
  {
    result.t = NO_INTERSECTION;
    result.intersection = O + V;
    result.slide = Vector2D(0.0, 0.0);
  }
  else
  {
    if (lineT < startT && lineT < endT)
    {
      lineT -= EPSILON;
      result.t = lineT;
      result.intersection = O + lineT * V;
      Vector2D W = E - S;
      result.slide = (((1-result.t) * V).dot(W) / W.length2()) * W;
    }
    else if (startT < lineT && startT < endT)
    {
      startT -= EPSILON;
      result.t = startT;
      result.intersection = O + startT * V;
      Vector2D W = calcNormal(S, result.intersection);
      result.slide = (W.dot((1 - startT) * V) * W);
    }
    else
    {
      endT -= EPSILON;
      result.t = endT;
      result.intersection = O + endT * V;
      Vector2D W = calcNormal(E, result.intersection);
      result.slide = (W.dot((1 - endT) * V) * W);
    }
  }
  return result;
}

Point2D Line::getStartVertex()
{
  return S;
}

Point2D Line::getEndVertex()
{
  return E;
}

bool Line::isInFront(Point2D pos)
{
  return (pos - S).dot(N) >= 0;
}

bool Line::isInFront(Point2D pos, double radius)
{
  return (pos - S).dot(N) + radius >= 0;
}

bool Line::isIntersecting(Point2D pos, double r, Collidable* other)
{
  if (this == other) return false;
  double dist = (pos - S).dot(N);
  if (std::abs(dist) > r) return false;
  double Sdist = (pos-S).length2();
  double Edist = (pos-E).length2();
  if ((E-S).dot(pos-S) < 0) return Sdist <= r * r;
  if ((S-E).dot(pos-E) < 0) return Edist <= r * r;
  return true;
}

bool Line::isBackfaceOf(Line other)
{
  return this->S[0] == other.E[0]
      && this->S[1] == other.E[1]
      && this->E[0] == other.S[0]
      && this->E[1] == other.S[1];
}

bool operator<(Collision a, Collision b)
{
  return a.t < b.t;
}

std::ostream& Line::operator<<(std::ostream& out)
{
  out << "[" << S << "->" << E << "]";
  return out;
}

void Line::reflectMVMatrix(Point2D orig)
{
  double wallAngleRad = atan2(S[1] - E[1], S[0] - E[0]);
  double wallAngleDeg = wallAngleRad * 180.0 / M_PI;
  Point3D middle(S[0] + E[0] / 2, S[1] + E[1] / 2, 0);
  glTranslated(E[0], E[1], 0);
  glRotated(wallAngleDeg, 0, 0, 1);
  glScaled(1, -1, 1);
  glRotated(-wallAngleDeg, 0, 0, 1);
  glTranslated(-E[0], -E[1], 0);
}

Point2D Line::reflectPoint(Point2D orig)
{
  double dist = (orig - S).dot(N);
  return orig + -2 * dist * N;
}

bool Line::clip(Point2D pos, Point2D left, Point2D right)
{
  Line leftFrustum(left, pos);
  Line rightFrustum(pos, right);

  if (!clip(leftFrustum)) return false;
  if (!clip(rightFrustum)) return false;
  return true;
}

bool Line::clip(Line clippingPlane)
{
  bool startFront = clippingPlane.isInFront(S);
  bool endFront = clippingPlane.isInFront(E);

  if (!startFront && !endFront) return false;
  if (startFront && endFront) return true;
  if (startFront) E = intersectLines(S, E, clippingPlane.getStartVertex(), clippingPlane.getEndVertex());
  if (endFront) S = intersectLines(S, E, clippingPlane.getStartVertex(), clippingPlane.getEndVertex());
  return true;
}

bool Line::clip(std::vector<Point2D>& vertices)
{
  std::vector<Point2D> result;
  Point2D lastPoint = vertices[vertices.size() - 1];
  bool lastOrientation = isInFront(lastPoint);

  for (std::vector<Point2D>::iterator iter = vertices.begin(); iter != vertices.end(); iter++)
  {
    Point2D currPoint = *iter;
    bool currOrientation = isInFront(currPoint);
    if (lastOrientation && currOrientation)
      result.push_back(currPoint);
    else if (!lastOrientation && !currOrientation) {}
    else if (lastOrientation && !currOrientation)
    {
      result.push_back(intersectLines(S, E, currPoint, lastPoint));
    }
    else if (!lastOrientation && currOrientation)
    {
      result.push_back(intersectLines(S, E, currPoint, lastPoint));
      result.push_back(currPoint);
    }
    lastOrientation = currOrientation;
    lastPoint = currPoint;
  }

  vertices = result;
  return result.size() > 0;
}

Vector3D Line::getRicochetVector()
{
  Vector3D P(E - S);
  P.normalize();
  Vector3D V(0, 0, 1);
  Vector3D A(N);
  Vector3D result =
    (0.5 - mathRandom()) * P +
    (0.5 - mathRandom()) * V +
    (0.5 + 1.5 * mathRandom()) * A;
  result.normalize();
  result = (10 + 10 * mathRandom()) * result;
  return result;
}

Matrix4x4 Line::getTransformation(Line& other)
{
  Vector2D trans = other.S - E;
  return translationMatrix(trans[0], trans[1], 0);
}

Vector3D Line::getNormal()
{
  return Vector3D(N[0], N[1], 0);
}
