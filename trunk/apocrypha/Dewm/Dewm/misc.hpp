#ifndef __MISC__
#define __MISC__

#include "algebra.hpp"
#include "line.hpp"
#include "entity.hpp"
#include <vector>

class Sector;

#define NO_INTERSECTION 500000.0
#define NUM_REFLECTIONS 5
#define EPSILON 1e-12
#define MIN_2D_SCALE 50
#define SPLIT_WALLS_INTO_LEVELS 1
#define STEP_HEIGHT 2
#define GRAVITY 32
#define NO_TELEPORTAL_REF '%'
#define MAX_CHASE_CAM_DIST 10

extern bool STENCIL_ENABLED;

enum HitscanResult { NONE, ENTITY, WALL, FLOOR, CEILING };

Point2D intersectLines(Point2D S1, Point2D E1, Point2D S2, Point2D E2);
double intersectLineSegment(Point2D p, Vector2D v, Point2D A, Point2D B);
double intersectCircle(Point2D p, Vector2D v, Point2D c, double r);
Vector2D calcNormal(Point2D start, Point2D end);
Matrix4x4 rotationAboutXMatrix(double angle);
Matrix4x4 rotationAboutYMatrix(double angle);
Matrix4x4 rotationAboutZMatrix(double angle);
Matrix4x4 scaleMatrix(double x, double y, double z);
Matrix4x4 translationMatrix(double x, double y, double z);
void addBulletEffect(HitscanResult result, Point3D pos, Sector* sector, void* victim, bool paintballMode);
double mathRandom();
void createFrustum(double FOV, Point2D pos, double angle, Point2D& left, Point2D& right);

#endif
