#include "misc.hpp"
#include "map.hpp"
#include "polyroots.hpp"
#include <stdlib.h>
#include <vector>

bool STENCIL_ENABLED;

double intersectCircle(Point2D P, Vector2D V, Point2D C, double R)
{
  Vector2D D = P - C;
  
  double a = V.length2();
  double b = 2 * V.dot(D);
  double c = D.length2() - R * R;

  double roots[2];

  int numRoots = quadraticRoots(a, b, c, roots);
  if (numRoots == 0) return NO_INTERSECTION;
  double minRoot = (numRoots == 1 ? roots[0] : std::min(roots[0], roots[1]));
  if (minRoot > 1 || minRoot < 0) return NO_INTERSECTION;
  return minRoot;
}

Point2D intersectLines(Point2D S1, Point2D E1, Point2D S2, Point2D E2)
{
  Vector2D N = calcNormal(S2, E2);
  double T = N.dot(S2 - S1) / N.dot(E1 - S1);
  Point2D result = S1 + T * (E1 - S1);
  return result;
}

double intersectLineSegment(Point2D O, Vector2D V, Point2D A, Point2D B)
{
  // Check that the velocity isn't parallel to the wall
  Vector2D N = calcNormal(A, B);
  if (std::abs(N.dot(V)) == 0)
  {
    //std::cout << "[Class 1]";
    return NO_INTERSECTION;
  }

  // Check that the velocity intersects the wall's line
  double T = N.dot(A - O) / N.dot(V);
  if (T > 1 || T < 0)
  {
    //std::cout << "[Class 2:" << T << "]";
    return NO_INTERSECTION;
  }

  // Check that the velocity intersects the wall's line SEGMENT
  Point2D I = O + T * V;
  if ((B-A).dot(I-A) < 0 || (A-B).dot(I-B) < 0)
  {
    //std::cout << "[Class 3]";
    return NO_INTERSECTION;
  }

  // We've got an actual intersection
  if (T < EPSILON) T = 0;
  //std::cout << "[Hit:" << T << "]";
  return T;
}

Vector2D calcNormal(Point2D start, Point2D end)
{
  Vector2D result(start[1] - end[1], end[0] - start[0]);
  result.normalize();
  return result;
}

Matrix4x4 rotationAboutXMatrix(double angle)
{
  return Matrix4x4(Vector4D(1,          0,           0, 0),
                   Vector4D(0, cos(angle), sin(-angle), 0),
                   Vector4D(0, sin(angle),  cos(angle), 0),
                   Vector4D(0,          0,           0, 1));
}

Matrix4x4 rotationAboutYMatrix(double angle)
{
  return Matrix4x4(Vector4D( cos(angle), 0, sin(angle), 0),
                   Vector4D(          0, 1,          0, 0),
                   Vector4D(sin(-angle), 0, cos(angle), 0),
                   Vector4D(          0, 0,          0, 1));
}

Matrix4x4 rotationAboutZMatrix(double angle)
{
  return Matrix4x4(Vector4D(cos(angle), sin(-angle), 0, 0),
                   Vector4D(sin(angle),  cos(angle), 0, 0),
                   Vector4D(         0,           0, 1, 0),
                   Vector4D(         0,           0, 0, 1));
}

Matrix4x4 scaleMatrix(double x, double y, double z)
{
  return Matrix4x4(Vector4D(x, 0, 0, 0),
                   Vector4D(0, y, 0, 0),
                   Vector4D(0, 0, z, 0),
                   Vector4D(0, 0, 0, 1));
}

Matrix4x4 translationMatrix(double x, double y, double z)
{
  return Matrix4x4(Vector4D(1, 0, 0, x),
                   Vector4D(0, 1, 0, y),
                   Vector4D(0, 0, 1, z),
                   Vector4D(0, 0, 0, 1));
}

double mathRandom()
{
  return (double)rand() / (double)RAND_MAX;
}

Vector3D getFloorRicochetVector()
{
  Vector3D Z(0, 0, 1);
  Vector3D X(1, 0, 0);
  Vector3D Y(0, 1, 0);
  Vector3D result =
    (0.5 - mathRandom()) * X +
    (0.5 - mathRandom()) * Y +
    (0.5 + 0.5 * mathRandom()) * Z;
  result.normalize();
  result = (10 + 10 * mathRandom()) * result;
  return result;
}

Vector3D getCeilingRicochetVector()
{
  Vector3D Z(0, 0, 1);
  Vector3D X(1, 0, 0);
  Vector3D Y(0, 1, 0);
  Vector3D result =
    (0.5 - mathRandom()) * X +
    (0.5 - mathRandom()) * Y +
    -(0.5 + 0.5 * mathRandom()) * Z;
  result.normalize();
  result = (10 + 10 * mathRandom()) * result;
  return result;
}

void addBulletEffect(HitscanResult result, Point3D pos, Sector* sector, void* victim, bool paintballMode)
{
  int particleCount = 6;
  Colour sparkColour(1.0, 0.75, 0);
  Colour holeColour(0.0, 0.0, 0.0);
  if (paintballMode)
  {
    sparkColour = Colour(0.5 + 0.5 * mathRandom(), 0.5 + 0.5 * mathRandom(), 0.5 + 0.5 * mathRandom());
    holeColour = sparkColour;
  }

  switch(result)
  {
  case(NONE): break;
  case(WALL):
    for (int count = 0; count < particleCount; count++)
      sector->addParticle(new TraceParticle(pos, ((Line*)victim)->getRicochetVector(), 0.5, (((double)count) / (particleCount - 1.0)) * sparkColour));
    sector->addParticle(new BulletHole(BulletHole::WALL, pos, ((Line*)victim)->getNormal(), 10, 0.1, holeColour));
    break;
  case(FLOOR):
    for (int count = 0; count < particleCount; count++)
      sector->addParticle(new TraceParticle(pos, getFloorRicochetVector(), 0.5, (((double)count) / (particleCount - 1.0)) * sparkColour));
    sector->addParticle(new BulletHole(BulletHole::FLOOR, pos, Vector3D(0,0,1), 10, 0.1, holeColour));
    break;
  case(CEILING):
    if (sector->hasSkyRoof()) break;
    for (int count = 0; count < particleCount; count++)
      sector->addParticle(new TraceParticle(pos, getCeilingRicochetVector(), 0.5, (((double)count) / (particleCount - 1.0)) * sparkColour));
    sector->addParticle(new BulletHole(BulletHole::CEILING, pos, Vector3D(0,0,-1), 10, 0.1, holeColour));
    break;
  case(ENTITY):
    for (int count = 0; count < particleCount; count++)
      sector->addParticle(new QuakeParticle(pos, Vector3D(mathRandom(), mathRandom(), mathRandom()), 0.5, (((double)count) / (particleCount + 1.0)) * Colour(1.0, 0.0, 0.0)));
    break;
  }
}

void createFrustum(double FOV, Point2D pos, double angle, Point2D& left, Point2D& right)
{
  Vector2D leftCalc(0, 100);
  Vector2D rightCalc(0, 100);
  leftCalc.rotateZ((angle + FOV / 2) * M_PI / 180);
  rightCalc.rotateZ((angle - FOV / 2) * M_PI / 180);
  left = pos + leftCalc;
  right = pos + rightCalc;
}
