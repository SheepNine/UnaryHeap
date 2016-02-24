#include "drawable.hpp"

Drawable::~Drawable() {}

CompareDrawableDepth::CompareDrawableDepth(Point2D pos): refPos(pos){}
bool CompareDrawableDepth::operator() (const Drawable* b1, const Drawable* b2) const
{
  if (b1->drawFirst() && !b2->drawFirst())
    return true;
  if (!b1->drawFirst() && b2->drawFirst())
    return false;
  return (refPos - b1->getPosition().collapseToXY()).length2()
    > (refPos - b2->getPosition().collapseToXY()).length2();
}
