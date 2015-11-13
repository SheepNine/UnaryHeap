#include "player.hpp"
#include "misc.hpp"
#include "particle.hpp"
#include "projectile.hpp"
#include "dlist.hpp"
#include "map.hpp"

Player::Player(Point2D pos, double a, Sector* s): Entity(pos, a, s, 1, 5)
{
  speed = 12;
  lookangle = 0;
  weapon = 2;
  zVel = 0;
  cameraDist = 0;
  cameraAngle = angle;
  desiredCameraDist = 0;
  displayHealth = health;
}

Player::~Player()
{
}

void Player::rotate(double delta)
{
  cameraAngle += delta;
  if (isAlive()) angle += delta;
}

void Player::look(double delta)
{
  lookangle += delta;
  if (lookangle > 45) lookangle = 45;
  if (lookangle < -45) lookangle = -45;
}

void Player::move(Vector2D d)
{
  if (isAlive()) dir = d;
}


void Player::render2D()
{
  glColor3f(1.0f, 1.0f, 1.0f);
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
      glVertex3d(x+position[0], y+position[1], 0);
    }
  }
  glEnd();
}

void Player::render3D(Point2D pos, Point2D left, Point2D right)
{
  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
  GLUquadric* q = gluNewQuadric();
  glPushMatrix();
  glTranslated(position[0], position[1], position[2]);
  glRotated(angle, 0, 0, 1);
  glColor3d(0.2, 0.2, 0.2);
  gluSphere(q, radius, 8, 8);
  glTranslated(0.0, 0.0, height - radius);
  glColor3d(1.0, 0.75, 0.5);
  gluSphere(q, radius * 0.75, 8, 8);
  glColor3d(1.0, 0.65, 0.4);
  glTranslated(0.0, radius / 2, 0.0);
  gluSphere(q, radius / 2, 8, 8);

  gluDeleteQuadric(q);
  glPopMatrix();
}

void Player::render3DForCamera()
{
  if (cameraDist < radius) return;
  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
  GLUquadric* q = gluNewQuadric();
  glPushMatrix();
  glTranslated(position[0], position[1], position[2]);
  glRotated(angle, 0, 0, 1);
  glColor4d(0.2, 0.2, 0.2, cameraDist / MAX_CHASE_CAM_DIST);
  gluSphere(q, radius, 8, 8);
  glTranslated(0.0, 0.0, height - radius);
  glColor4d(1.0, 0.75, 0.5, cameraDist / MAX_CHASE_CAM_DIST);
  gluSphere(q, radius * 0.75, 8, 8);
  glColor4d(1.0, 0.65, 0.4, cameraDist / MAX_CHASE_CAM_DIST);
  glTranslated(0.0, radius / 2, 0.0);
  gluSphere(q, radius / 2, 8, 8);

  gluDeleteQuadric(q);
  glPopMatrix();
}

void Player::renderScene2D(Map* map)
{
  map->render2D(position.collapseToXY(), angle);
  /*double FOV = 45;
  glMatrixMode(GL_PROJECTION);
  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
  glLoadIdentity();
  GLint viewport[4];
  glGetIntegerv(GL_VIEWPORT, viewport);
  double ar = (double)viewport[2] / (double)viewport[3];
  glOrtho(-MIN_2D_SCALE * ar, MIN_2D_SCALE * ar, -MIN_2D_SCALE, MIN_2D_SCALE, 0, 10000);
  glMatrixMode(GL_MODELVIEW);
  glLoadIdentity();

  Vector2D leftCalc(100, 0);
  Vector2D rightCalc(100, 0);
  leftCalc.rotateZ((angle + FOV) * M_PI / 180);
  rightCalc.rotateZ((angle - FOV) * M_PI / 180);
  Point2D left = position.collapseToXY() + leftCalc;
  Point2D right = position.collapseToXY() + rightCalc;

  glRotated(-angle, 0,0,1);
  glTranslated(-position[0], -position[1], 0);

  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
  glColor3d(0.0, 1.0, 1.0);
  glBegin(GL_LINES);
  {
    glVertex3d(0.0,0.0,0.0);
    glVertex3d(10.0,0.0,0.0);

    glVertex3d(0.0,0.0,0.0);
    glVertex3d(0.0,10.0,0.0);
  }
  glEnd();

  glDisable(GL_LIGHTING);
  currSector->render2D();
  glEnable(GL_LIGHTING);*/
}

void Player::renderScene3D()
{
  double FOV = 45;
  glMatrixMode(GL_PROJECTION);
  glLoadIdentity();
  GLint viewport[4];
  glGetIntegerv(GL_VIEWPORT, viewport);
  double ar = (double)viewport[2] / (double)viewport[3];
  gluPerspective(FOV * 1.5, ar, 0.001, 10000);
  glMatrixMode(GL_MODELVIEW);
  glLoadIdentity();

  Vector3D cameraBack(0, -cameraDist, 0);

  cameraBack = rotationAboutZMatrix(cameraAngle * M_PI / 180) * rotationAboutXMatrix(lookangle * M_PI / 180) * cameraBack;
  Sector* cameraSector = NULL;
  Point3D cameraPos;
  void* dontCare;
  double T;
  HitscanResult result = currSector->resolveHitscan(position + Vector3D(0,0,height - radius), cameraBack, true, STENCIL_ENABLED, T, cameraPos, cameraSector, dontCare);
  if (result == FLOOR)
    cameraPos = cameraPos + Vector3D(0, 0, 0.1);
  else if (result == CEILING)
    cameraPos = cameraPos + Vector3D(0, 0, -0.1);
  else if (result == WALL)
    cameraPos = cameraPos + 0.1 * ((Line*)dontCare)->getNormal();
  cameraDist = cameraBack.length() * T;

  Point2D pos = cameraPos.collapseToXY(), left, right;
  createFrustum(180, pos, cameraAngle, left, right);

  glRotated(-(lookangle + 90), 1, 0, 0);
  glRotated(-cameraAngle, 0, 0, 1);
  glTranslated(-cameraPos[0], -cameraPos[1], -cameraPos[2]);

  if (!STENCIL_ENABLED)
  {
    glCallList(displayListHandles[SKYBOX_LIST]);
  }
  cameraSector->render3D(pos, left, right, NUM_REFLECTIONS);
  render3DForCamera();
}

void Player::renderSBar()
{
  glMatrixMode(GL_PROJECTION);
  glLoadIdentity();
  GLint viewport[4];
  glGetIntegerv(GL_VIEWPORT, viewport);
  glOrtho(0, viewport[2], 0, viewport[3], 0, 10000);
  glMatrixMode(GL_MODELVIEW);
  glLoadIdentity();

  glDisable(GL_LIGHTING);

  glColor3d(0.8, 0.8, 0.8);
  glBindTexture(GL_TEXTURE_2D, textureHandles[PANEL_TEXTURE]);
  glBegin(GL_TRIANGLES);
  {
    glTexCoord2d(1.0, 0.0);
    glVertex3d(viewport[2], 0, 0);
    glTexCoord2d(1.0, 1.0);
    glVertex3d(viewport[2], viewport[3] / 10.0, 0);
    glTexCoord2d(0.0, 0.0);
    glVertex3d(0, 0, 0);

    glTexCoord2d(0.0, 0.0);
    glVertex3d(0, 0, 0);
    glTexCoord2d(1.0, 1.0);
    glVertex3d(viewport[2], viewport[3] / 10.0, 0);
    glTexCoord2d(0.0, 1.0);
    glVertex3d(0, viewport[3] / 10.0, 0);
  }
  glEnd();

  double minX = viewport[2] / 40.0;
  double widthX = viewport[2] * 38.0 / 40.0;
  double minY = viewport[3] / 40.0;
  double maxY = viewport[3] * 3.0 / 40.0;

  glColor3d(0.0,0.0,0.0);
  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);

  glBegin(GL_TRIANGLES);
  {
    glVertex3d(minX + widthX, minY, 0.0);
    glVertex3d(minX + widthX, maxY, 0.0);
    glVertex3d(minX, minY, 0.0);

    glVertex3d(minX, minY, 0.0);
    glVertex3d(minX + widthX, maxY, 0.0);
    glVertex3d(minX, maxY, 0.0);
  }
  glEnd();

  glBegin(GL_TRIANGLES);
  {
    glColor3d(0.0,0.8,0.0);
    glVertex3d(minX + (widthX*health) - 1, minY+1, 0.0);
    glVertex3d(minX + (widthX*health) - 1, maxY-1, 0.0);
    glVertex3d(minX+1, minY+1, 0.0);

    glVertex3d(minX+1, minY+1, 0.0);
    glVertex3d(minX + (widthX*health) - 1, maxY-1, 0.0);
    glVertex3d(minX+1, maxY-1, 0.0);

    glColor3d(0.8,0.0,0.0);

    glVertex3d(minX + (widthX*displayHealth) - 1, minY+1, 0.0);
    glVertex3d(minX + (widthX*displayHealth) - 1, maxY-1, 0.0);
    glVertex3d(minX + (widthX*health) - 1, minY+1, 0.0);

    glVertex3d(minX + (widthX*health) - 1, minY+1, 0.0);
    glVertex3d(minX + (widthX*displayHealth) - 1, maxY-1, 0.0);
    glVertex3d(minX + (widthX*health) - 1, maxY-1, 0.0);
  }
  glEnd();

  glColor4d(1.0, 0.0, 0.0, 2.0 * (displayHealth - health));
  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
  glBegin(GL_QUADS);
  {
    glVertex3d(0.0, 0.0, 0.0);
    glVertex3d(viewport[2], 0.0, 0.0);
    glVertex3d(viewport[2], viewport[3], 0.0);
    glVertex3d(0.0, viewport[3], 0.0);
  }
  glEnd();

  glEnable(GL_LIGHTING);
}

void Player::fire(bool paintballMode)
{
  if (cooldown == 0)
  {
    cooldown = 0.5;
    Vector3D trajectory(0, 1, 0);
    Vector3D offsetX(1, 0, 0);
    Vector3D offsetY(0, 0, 1);
    Matrix4x4 rot = rotationAboutZMatrix(angle * M_PI / 180.0) * rotationAboutXMatrix(lookangle * M_PI / 180.0);
    trajectory = rot * trajectory;
    offsetX = rot * offsetX;
    offsetY = rot * offsetY;

    Sector* destSector;
    void* victim;
    Point3D dest;
    double dontCare;
    if (weapon == 0)
    {
      cooldown = 0.4;
      destSector = NULL;
      HitscanResult result = currSector->resolveHitscan(
          position + Vector3D(0, 0, height-radius),
          4 * radius * trajectory,
          true, true, dontCare, dest, destSector, victim);
      if (result == ENTITY)
        ((Entity*)victim)->damage(0.05);
      addBulletEffect(result, dest, destSector, victim, paintballMode);
    }
    else if (weapon == 1)
    {
      cooldown = 0.5;
      for (int count = 0; count < 8; count++)
      {
        destSector = NULL;
        HitscanResult result = currSector->resolveHitscan(
          position + Vector3D(0, 0, height-radius),
          1000 * trajectory + (50 - 100 * mathRandom()) * offsetX + (50 - 100 * mathRandom()) * offsetY,
          true, true, dontCare, dest, destSector, victim);
        if (result == ENTITY)
          ((Entity*)victim)->damage(0.05);
        addBulletEffect(result, dest, destSector, victim, paintballMode);
      }
    }
    else if (weapon == 2)
    {
      cooldown = 0.1;
      destSector = NULL;
      HitscanResult result = currSector->resolveHitscan(
          position + Vector3D(0, 0, height-radius),
          1000 * trajectory + (25 - 50 * mathRandom()) * offsetX + (25 - 50 * mathRandom()) * offsetY,
          true, true, dontCare, dest, destSector, victim);
      if (result == ENTITY)
        ((Entity*)victim)->damage(0.05);
      addBulletEffect(result, dest, destSector, victim, paintballMode);
    }
    else if (weapon == 3)
    {
      cooldown = 1.0;
      currSector->addProjectile(new Rocket(position + Vector3D(0, 0, height - radius), 32 * trajectory, currSector));
    }
  }
}

void Player::update(double time)
{
  int nearestFloor,nearestCeiling;
  if (health > 0)
  {
    if (dir.length2() > 0)
    {
      double theta = angle * M_PI / 180.0;
      dir.rotateZ(theta);
      Vector2D vel = time * speed * dir;

      nearestFloor = INT_MIN; nearestCeiling = INT_MAX;
      currSector->nearestHeights(position.collapseToXY(), radius, nearestFloor, nearestCeiling, this);

      vel = currSector->resolveCollisions(position, vel, radius, height, position[2] == nearestFloor, this);
      Point3D dest = Point3D(position[0] + vel[0], position[1] + vel[1], position[2]);
      Sector* origSector = currSector;
      Sector* destSector = currSector->getDestSector(position.collapseToXY(), dest);
      position = dest;
      if (origSector != destSector)
      {
        currSector->removeEntity(this);
        currSector = destSector;
        currSector->addEntity(this);
      }
      dir = Vector2D(0,0);
    }
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

  if (health > 0)
  {
    cooldown -= time;
    if (cooldown < 0) cooldown = 0;
  }
  else
  {
    cooldown = 1;
    if (height > 2 * radius) height -= time * GRAVITY;
    if (height < 2 * radius) height = 2 * radius;
    desiredCameraDist = MAX_CHASE_CAM_DIST;
  }

  if (cameraDist > desiredCameraDist) cameraDist -= 0.4;
  if (cameraDist < desiredCameraDist) cameraDist += 0.4;
  if (displayHealth > health) displayHealth -= (time * 0.5);
}

void Player::switchWeapon(int w)
{
  weapon = w;
}

void Player::jump()
{
  if (!isAlive()) return;
  int nearestFloor = INT_MIN;
  int nearestCeiling = INT_MAX;
  currSector->nearestHeights(position.collapseToXY(), radius, nearestFloor, nearestCeiling, this);
  if (position[2] == nearestFloor)
  {
    zVel += 16;
  }
}

void Player::moveChaseCam(double delta)
{
  desiredCameraDist += delta;
  if (desiredCameraDist < 0) desiredCameraDist = 0;
  if (desiredCameraDist > MAX_CHASE_CAM_DIST) desiredCameraDist = MAX_CHASE_CAM_DIST;
}
