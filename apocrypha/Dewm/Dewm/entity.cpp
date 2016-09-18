#include "entity.hpp"
#include "sector.hpp"
#include "misc.hpp"
#include "polyroots.hpp"

Entity::~Entity() {}

Entity::Entity(Point2D pos, double a, Sector* s, double r, double h)
{
  position = Point3D(pos[0], pos[1], s->floorHeight());
  angle = a;
  currSector = s;
  radius = r;
  height = h;
  health = 1.0;
  cooldown = 0;
  recentlyDamaged = false;
  currSector->addEntity(this);
}

bool Entity::drawFirst() const { return false; }

bool Entity::isVisible(Point2D pos, Point2D left, Point2D right)
{
  Line leftFrustum(left, pos);
  Line rightFrustum(pos, right);

  return leftFrustum.isInFront(position.collapseToXY(), radius)
    && rightFrustum.isInFront(position.collapseToXY(), radius);
}

double Entity::resolveHitscan(Point3D source, Vector3D trace)
{
  if (health <= 0) return NO_INTERSECTION;
  Vector2D D = (source - position).collapseToXY();
  // Break if the shot starts inside me
  if (D.length() < radius) return NO_INTERSECTION;
  
  double a = trace.collapseToXY().length2();
  double b = 2 * trace.collapseToXY().dot(D);
  double c = D.length2() - radius * radius;

  double roots[2];
  int numRoots = quadraticRoots(a, b, c, roots);

  if (numRoots == 0) return NO_INTERSECTION;

  if (numRoots == 1) roots[1] = roots[0];
  if (roots[0] > roots[1])
  {
    double temp = roots[0];
    roots[0] = roots[1];
    roots[1] = temp;
  }
  if (roots[0] < 0 || roots[0] > 1) return NO_INTERSECTION;

  double height1 = (source + roots[0] * trace)[2];
  double height2 = (source + roots[1] * trace)[2];
  if (height1 < position[2] && height2< position[2] ||
    height1 > position[2] + height && height1 > position[2] + height)
    return NO_INTERSECTION;
  return roots[0];
}

void Entity::damage(double amount)
{
  recentlyDamaged = true;
  health -= amount;
  if (health < 0) health = 0;
}

Collision Entity::collide(Point2D o, Vector2D v, double r)
{
  Collision result;
  if (health <= 0)
  {
    result.intersection = o + v;
    result.slide = Vector2D(0,0);
    result.t = NO_INTERSECTION;
    return result;
  }
  double T = intersectCircle(o, v, position.collapseToXY(), r + radius);
  if (T == NO_INTERSECTION)
  {
    result.intersection = o + v;
    result.slide = Vector2D(0,0);
    result.t = NO_INTERSECTION;
  }
  else
  {
    result.t = T;
    result.intersection = o + T * v;
    Vector2D W = calcNormal(position.collapseToXY(), result.intersection);
    result.slide = (W.dot((1 - T) * v) * W);
  }

  return result;
}

bool Entity::isIntersecting(Point2D p, double r, Collidable* other)
{
  if (this == other) return false;
  if (health <= 0) return false;
  return (position.collapseToXY() - p).length() <= r + radius;
}

Point3D Entity::getPosition() const
{
  return position;
}

bool Entity::isAlive()
{
  return health > 0;
}

Enemy::Enemy(Point2D pos, double angle, Sector *s, Entity* target)
:Entity(pos, angle, s, 1, 5), player(target), zVel(0), lastKnownView(0, 0, 0)
{
}

Enemy::~Enemy() {}

void Enemy::render2D()
{
  if (health > 0)
    glColor3d(0.0f, 1.0f, 0.0f);
  else
    glColor3d(0.0, 0.0, 0.0);
  Vector2D facing(0, 1);
  facing.rotateZ(angle * M_PI / 180);
  facing = radius * facing;
  Point2D facingPoint = position.collapseToXY() + facing;
  glBegin(GL_LINES);
  {
    glVertex3d(position[0], position[1], 0.0);
    glVertex3d(facingPoint[0], facingPoint[1], 0.0);
  }
  glEnd();
  glBegin(GL_LINE_LOOP);
  {
    for (int count = 0; count < 16; count++)
    {
      double theta = M_PI * (double)count / 8.0;
      double x = radius * cos(theta);
      double y = radius * sin(theta);
      glVertex3d(x + position[0], y + position[1], 0);
    }
  }
  glEnd();

  if (isAlive())
  {
    glColor3d(0.0, 1.0, 1.0);
    glBegin(GL_LINES);
    {
      glVertex3d(position[0], position[1], 0.0);
      glVertex3d(position[0] + lastKnownView[0], position[1] + lastKnownView[1], 0.0);
    }
    glEnd();
  }
}

void Enemy::render3D(Point2D pos, Point2D left, Point2D right)
{
  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
  GLUquadric* q = gluNewQuadric();
  glPushMatrix();
  glTranslated(position[0], position[1], position[2]);
  glRotated(angle, 0, 0, 1);
  glColor3d(0.2, 0.2, 0.2);
  gluSphere(q, radius, 8, 8);
  glTranslated(0.0, 0.0, height - radius);
  glColor3d(1.0, 0.2, 0.2);
  gluSphere(q, radius * 0.8, 8, 8);
  glColor3d(1.0, 0.1, 0.1);
  glTranslated(0.0, radius / 2, 0.0);
  gluSphere(q, radius / 2, 8, 8);

  gluDeleteQuadric(q);
  glPopMatrix();
}

void Enemy::update(double time)
{
  if (health <= 0)
  {
    if (height > 2 * radius) height -= time * GRAVITY;
    if (height < 2 * radius) height = 2 * radius;
  }
  else if (player->isAlive())
  {
    if (cooldown > 0)
      cooldown -= time;
    Point2D left, right;
    createFrustum(180, position.collapseToXY(), angle, left, right);
    Vector3D view = currSector->findEntity(position.collapseToXY(), left, right, player, NUM_REFLECTIONS);
    if (view[0] == 0 && view[0] == 0 && recentlyDamaged) // If I've been hurt, autoacquire the player
      view = currSector->findEntity(position.collapseToXY(), right, left, player, NUM_REFLECTIONS);
    bool canSeePlayer = (view[0] != 0 || view[1] != 0);

    if (canSeePlayer)
    {
      if (cooldown <= 0)
      {
        cooldown = 1.5 + mathRandom();
        Vector3D trajectory(view);
        trajectory.normalize();
        trajectory = 20 * trajectory;
        currSector->addProjectile(new Fireball(position + Vector3D(0, 0, height-radius), trajectory, currSector));
      }
      lastKnownView = view;
    }
    if (lastKnownView[0] != 0 || lastKnownView[1] != 0)
      angle = atan2(lastKnownView[1], lastKnownView[0]) * 180.0 / M_PI  - 90;

    int nearestFloor, nearestCeiling;
    if (canSeePlayer && lastKnownView.length() > 10 ||
      !canSeePlayer && lastKnownView.length() > 1)
    {
      Vector2D vel = lastKnownView.collapseToXY();
      vel.normalize();
      vel = 7 * time * vel;

      nearestFloor = INT_MIN; nearestCeiling = INT_MAX;
      currSector->nearestHeights(position.collapseToXY(), radius, nearestFloor, nearestCeiling, this);

      vel = currSector->resolveCollisions(position, vel, radius, height, position[2] == nearestFloor, this);
      Point3D dest(position[0] + vel[0], position[1] + vel[1], position[2]);
      Sector* origSector = currSector;
      Sector* destSector = currSector->getDestSector(position.collapseToXY(), dest);
      position = dest;
      if (origSector != destSector)
      {
        currSector->removeEntity(this);
        currSector = destSector;
        currSector->addEntity(this);
      }

      lastKnownView = lastKnownView - Vector3D(vel[0], vel[1], 0);
    }

    position[2] += zVel * time;
    zVel -= GRAVITY * time;

    nearestFloor = INT_MIN; nearestCeiling = INT_MAX;
    currSector->nearestHeights(position.collapseToXY(), radius, nearestFloor, nearestCeiling, this);


    if (position[2] + height > nearestCeiling)
    {
      position[2] = nearestCeiling - height;
      zVel = 0;
    }
    if (position[2] <= nearestFloor)
    {
      position[2] = nearestFloor;
      zVel = 0;
    }

    cooldown -= time;
    if (cooldown < 0) cooldown = 0;

    recentlyDamaged = false;
  }
}
