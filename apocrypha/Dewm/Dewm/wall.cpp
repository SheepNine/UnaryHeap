#include "wall.hpp"
#include "sector.hpp"
#include "SDL_opengl.h"
#include "texture.hpp"
#include "misc.hpp"
#include "dlist.hpp"

Wall::Wall(Point2D start, Point2D end, TextureSet tex, std::string flags, char ref):L(start, end), T(tex), portalRef(ref)
{
  B = NULL;
  F = NULL;
  isMirror = flags.find("M") != std::string::npos;
  isTeleportal = false;
  isPortal = false;
  if (isMirror) T.middle = WallTexture::getMirrorTexture();
}

void Wall::render2D()
{
  if (isPortal) glColor3d(0.7, 0.7, 0.0);
  else if (isTeleportal) glColor3d(1.0, 0.0, 1.0);
  else glColor3d(0.8, 0.0, 0.0);

  L.render2D();
}

Vector3D Wall::findEntity(Point2D pos, Point2D left, Point2D right, Entity* target, unsigned int portalDepth)
{
  if (!isPortal && !isTeleportal) return Vector2D(0,0);
  if (!L.isInFront(pos)) return Vector2D(0,0);
  Line clippedWall(L);
  if (!clippedWall.clip(pos, left, right)) return Vector2D(0,0);

  if (portalDepth > 0)
  {
    if (isPortal)
      return B->findEntity(pos, clippedWall.getEndVertex(), clippedWall.getStartVertex(), target, portalDepth - 1) + Vector3D(0, 0, B->floorHeight() - F->floorHeight());
    else // if (isTeleportal)
      return B->findEntity((crossTransformation * pos.make3D()).collapseToXY(),
      (crossTransformation * clippedWall.getEndVertex().make3D()).collapseToXY(),
      (crossTransformation * clippedWall.getStartVertex().make3D()).collapseToXY(),
      target, portalDepth - 1) + Vector3D(0, 0, crossTransformation[2][3]);
  }
  else return Vector2D(0,0);
}

void Wall::render3D(Point2D pos, Point2D left, Point2D right, unsigned int bouncesLeft)
{
  if (!L.isInFront(pos)) return;
  Line clippedWall(L);
  if (!clippedWall.clip(pos, left, right)) return;
  //double clipTexOffset = (L.getEndVertex() - clippedWall.getEndVertex()).length() / 8.0;

  if (isMirror)
  {
    if (bouncesLeft > 0 && STENCIL_ENABLED)
    {
      drawStencil(F->floorHeight(), F->ceilingHeight());

      glPushMatrix();
      L.reflectMVMatrix(pos);
      GLint orientation;
      glGetIntegerv(GL_FRONT_FACE, &orientation);
      if (orientation == GL_CW)
        glFrontFace(GL_CCW);
      else
        glFrontFace(GL_CW);
      F->render3D(L.reflectPoint(pos), L.reflectPoint(right), L.reflectPoint(left), bouncesLeft - 1);
      glFrontFace(orientation);
      glPopMatrix();

      eraseStencil(F->floorHeight(), F->ceilingHeight());

      glColor4f(0.5f, 0.5f, 1.0f, 0.35f);
      glBindTexture(GL_TEXTURE_2D, textureHandles[MIRROR_TEXTURE]);
      L.render3D(F->floorHeight(), F->ceilingHeight(), 0, 0);
    }
    else
    {
      glColor4f(0.5f, 0.5f, 1.0f, 1.0f);
      glBindTexture(GL_TEXTURE_2D, textureHandles[MIRROR_TEXTURE]);
      L.render3D(F->floorHeight(), F->ceilingHeight(), 0, 0);
    }
  }
  else if (isTeleportal)
  {
    //int lipHeight = std::min(F->ceilingHeight() - F->floorHeight(), B->ceilingHeight() - B->floorHeight());
    int portalHeight = std::min(F->ceilingHeight() - F->floorHeight(), B->ceilingHeight() - B->floorHeight());
    int frontPortalHeight = std::min(F->floorHeight() + portalHeight, F->ceilingHeight());

    if (bouncesLeft > 0 && STENCIL_ENABLED)
    {
      drawStencil(F->floorHeight(), frontPortalHeight);

      glPushMatrix();
      glMultMatrixd(crossTransformation.invert().transpose()[0]);
      Point2D telePos = (crossTransformation * pos.make3D()).collapseToXY();
      Point2D teleLeft = (crossTransformation * left.make3D()).collapseToXY();
      Point2D teleRight = (crossTransformation * right.make3D()).collapseToXY();
      B->render3D(telePos, teleLeft, teleRight, bouncesLeft - 1);
      glPopMatrix();


      eraseStencil(F->floorHeight(), frontPortalHeight);

      glColor4f(1.0f, 0.0f, 1.0f, 0.35f);
      glBindTexture(GL_TEXTURE_2D, textureHandles[WATER_TEXTURE]);
      L.render3D(F->floorHeight(), frontPortalHeight, 0, 0);

      if (frontPortalHeight < F->ceilingHeight())
      {
        T.upper.apply();
        L.render3D(frontPortalHeight, F->ceilingHeight(), 0, 0);
      }
    }
    else
    {
      WallTexture::getPortalTexture().apply();
      L.render3D(F->floorHeight(), frontPortalHeight, 0, 0);

      if (frontPortalHeight < F->ceilingHeight())
      {
        T.upper.apply();
        L.render3D(frontPortalHeight, F->ceilingHeight(), 0, 0);
      }
    }
  }
  else if (isPortal)
  {
    int floor = std::max(F->floorHeight(), B->floorHeight());
    int ceiling = std::min(F->ceilingHeight(), B->ceilingHeight());

    drawStencil(floor, ceiling);
    B->render3D(pos, clippedWall.getEndVertex(), clippedWall.getStartVertex(), bouncesLeft);
    eraseStencil(floor, ceiling);

    if (F->floorHeight() < B->floorHeight())
    {
      T.lower.apply();
      L.render3D(F->floorHeight(), B->floorHeight(), 0, 0);
    }
    if (B->ceilingHeight() < F->ceilingHeight())
    {
      T.upper.apply();
      L.render3D(B->ceilingHeight(), F->ceilingHeight(), 0, 0);
    }
  }
  else
  {
    T.middle.apply();
    L.render3D(F->floorHeight(), F->ceilingHeight(), 0, 0);
  }
}

void Wall::drawStencil(int floor, int ceiling)
{
  glDepthMask(GL_FALSE); glDisable(GL_DEPTH_TEST);
  GLint stencilRef;
  glGetIntegerv(GL_STENCIL_REF, &stencilRef);
  glStencilFunc(GL_EQUAL, stencilRef, 0xFFFF);
  glStencilOp(GL_KEEP, GL_KEEP, GL_INCR);

  glColor4d(0.0, 0.0, 0.0, 0.0);
  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
  L.render3D(floor, ceiling, 0, 0);

  glStencilFunc(GL_EQUAL, stencilRef + 1, 0xFFFF);
  glStencilOp(GL_KEEP, GL_KEEP, GL_KEEP);

  glDepthMask(GL_TRUE); glEnable(GL_DEPTH_TEST);
}

void grayScreen()
{
  glDisable(GL_LIGHTING);
  glMatrixMode(GL_PROJECTION);
  glPushMatrix();
  glLoadIdentity();
  glOrtho(0, 1, 0, 1, 0, 10000);
  glMatrixMode(GL_MODELVIEW);
  glPushMatrix();
  glLoadIdentity();

  glColor4d(1.0, 1.0, 1.0, 0.1);
  glBegin(GL_QUADS);
  {
    glVertex2i(0, 0);
    glVertex2i(1, 0);
    glVertex2i(1, 1);
    glVertex2i(0, 1);
  }
  glEnd();

  glMatrixMode(GL_PROJECTION);
  glPopMatrix();
  glMatrixMode(GL_MODELVIEW);
  glPopMatrix();
  glEnable(GL_LIGHTING);
}

void Wall::eraseStencil(int floor, int ceiling)
{
  glDepthMask(GL_FALSE); glDisable(GL_DEPTH_TEST);
  //grayScreen();
  GLint stencilRef;
  glGetIntegerv(GL_STENCIL_REF, &stencilRef);
  glStencilFunc(GL_EQUAL, stencilRef, 0xFFFF);
  glStencilOp(GL_KEEP, GL_KEEP, GL_DECR);

  glColor4d(0.0, 0.0, 0.0, 0.0);
  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
  L.render3D(floor, ceiling, 0, 0);

  glStencilFunc(GL_EQUAL, stencilRef - 1, 0xFFFF);
  glStencilOp(GL_KEEP, GL_KEEP, GL_KEEP);

  glDepthMask(GL_TRUE); glEnable(GL_DEPTH_TEST);
}

void Wall::setFrontSector(Sector* s)
{
  F = s;
}

void Wall::link(Wall* other)
{
  if (L.isBackfaceOf(other->L))
  {
    if (isPortal || other->isPortal)
    {
      std::cerr << "Trying to portal a teleportal!" << std::endl;
      throw 0;
    }
    isPortal = true;
    other->isPortal = true;
    B = other->F;
    other->B = F;
  }
  if (this != other && this->portalRef == other->portalRef && this->portalRef != NO_TELEPORTAL_REF)
  {
    if (isPortal || other->isPortal)
    {
      std::cerr << "Trying to teleportal a portal!" << std::endl;
      throw 0;
    }
    isTeleportal = true;
    other->isTeleportal = true;
    B = other->F;
    other->B = F;
    this->linkTeleportal(other);
    other->linkTeleportal(this);
  }
}

void Wall::linkTeleportal(Wall* other)
{
  crossTransformation = crossTransformation * translationMatrix(0, 0, B->floorHeight() - F->floorHeight());
  crossTransformation = crossTransformation * L.getTransformation(other->L);
}

Point2D Wall::getStartVertex()
{
  return L.getStartVertex();
}

void Wall::getCollidables(Point3D pos, Vector2D vel, double radius, double height, std::vector<Collidable*>& result, bool canStep, Collidable* other)
{
  Point2D projPos = pos.collapseToXY();
  int stepHeight = canStep ? STEP_HEIGHT : 0;

  if (!isPortal && !isTeleportal)
  {
    result.push_back(&L);
  }
  else if (isPortal)
  {
    if (L.isInFront(projPos))
    {
      if (B->floorHeight() - pos[2] > stepHeight ||
          B->ceilingHeight() - B->floorHeight() < height ||
          B->ceilingHeight() - pos[2] < height)
      {
        result.push_back(&L);
      }
      else if (L.isIntersecting(projPos + vel, radius, other) ||
               L.isIntersecting(projPos, radius, other) ||
               !L.isInFront(projPos + vel))
      {
        B->getCollidables(pos, vel, radius, height, result, canStep, other);
      }
    }
  }
  else if (isTeleportal)
  {
    if (L.isInFront(projPos))
    {
      Point3D telePos = crossTransformation * pos;
      if (B->floorHeight() - telePos[2] > stepHeight ||
          B->ceilingHeight() - B->floorHeight() < height ||
          B->ceilingHeight() - telePos[2] < height)
      {
        result.push_back(&L);
      }
      else if (L.isIntersecting(projPos + vel, radius, other) ||
               L.isIntersecting(projPos, radius, other) ||
               !L.isInFront(projPos + vel))
      {
        B->getCollidables(telePos, vel, radius, height, result, canStep, other);
      }
    }
  }
}

void Wall::getEntities(Point2D pos, double radius, std::vector<Entity*>& result)
{
  if (L.isInFront(pos) && L.isIntersecting(pos, radius, NULL))
  {
    if (isPortal)
      B->getEntities(pos, radius, result);
    if (isTeleportal)
    {
      Point3D pos3 = pos.make3D();
      B->getEntities((crossTransformation * pos3).collapseToXY(), radius, result);
    }
  }
}

void Wall::nearestHeights(Point2D pos, double radius, int& floor, int& ceiling, Collidable* other)
{
  if (L.isInFront(pos) && L.isIntersecting(pos, radius, other))
  {
    if (isPortal)
      B->nearestHeights(pos, radius, floor, ceiling, other);
    else if (isTeleportal)
    {
      Point3D telePos = crossTransformation * pos.make3D();
      int heightDelta = (int)crossTransformation[2][3];
      floor += heightDelta;
      ceiling += heightDelta;
      B->nearestHeights(telePos.collapseToXY(), radius, floor, ceiling, other);
      floor -= heightDelta;
      ceiling -= heightDelta;
    }
  }
}

Sector* Wall::walkThrough(Point2D start, Point3D& end)
{
  if (L.isInFront(start) && !L.isInFront(end.collapseToXY()) &&
    intersectLineSegment(start, end.collapseToXY() - start, L.getStartVertex(), L.getEndVertex()) != NO_INTERSECTION)
  {
    if (isTeleportal) end = crossTransformation * end;
    return B;
  }
  return NULL;
}

std::ostream& Wall::operator<<(std::ostream& out)
{
  L << out;
  out << "(" << F << ")(" << B << ")[" << portalRef << "]" << isMirror;
  return out;
}

bool Wall::isInside(Point2D pos)
{
  return L.isInFront(pos);
}

HitscanResult Wall::resolveHitscan(Point3D pos, Vector3D trace, bool hitEntities, bool crossTeleportals, double& tVal, Point3D& dest, Sector* &camSector, void*& victim)
{
  if (!L.isInFront(pos.collapseToXY())) return NONE;
  double t = intersectLineSegment(pos.collapseToXY(), trace.collapseToXY(), L.getStartVertex(), L.getEndVertex());
  if (t == NO_INTERSECTION) return NONE;

  Point3D hit = pos + (t - EPSILON) * trace;

  if (!isPortal && !isTeleportal)
  {
    camSector = F;
    dest = hit;
    victim = &L;
    tVal = t;
    return WALL;
  }
  else if (isPortal)
  {
    if (hit[2] > B->ceilingHeight() || hit[2] < B->floorHeight())
    {
      camSector = F;
      dest = hit;
      victim = &L;
      tVal = t;
      return WALL;
    }
    else return B->resolveHitscan(pos, trace, hitEntities, crossTeleportals, tVal, dest, camSector, victim);
  }
  else// if (isTeleportal)
  {
    if (hit[2] - F->floorHeight() > std::min(F->ceilingHeight() - F->floorHeight(), B->ceilingHeight() - B->floorHeight())
      || !crossTeleportals)
    {
      camSector = F;
      dest = hit;
      victim = &L;
      tVal = t;
      return WALL;
    }
    else return B->resolveHitscan(crossTransformation * pos, trace, hitEntities, crossTeleportals, tVal, dest, camSector, victim);
  }
}
