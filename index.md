---
title: The Unary Heap
layout: baremetal-page
---

[Mug Muffins]({{ site.baseurl }}{% link recipes/mug-muffin.md %})

[Tex-Mex Seasoning]({{ site.baseurl }}{% link recipes/tex-mex.md %})

[Stuffed Cabbage Casserole]({{ site.baseurl }}{% link recipes/stuffed-cabbage-casserole.md %})

[Spaghetti Squash]({{ site.baseurl }}{% link recipes/spaghetti-squash.md %})

[TODO]({{ site.baseurl }}{% link pages/todo.md %})

{% for post in site.posts %}
## {{post.date | date: "%Y-%m-%d"}}: {{post.title}}
[View]({{ post.url | relative_url }}) {{ post.description }}
{% endfor %}

