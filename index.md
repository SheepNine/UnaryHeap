---
title: The Unary Heap
layout: baremetal-page
---

[Mug Muffins]({{ site.baseurl }}{% link recipes/mug-muffin.md %})

{% for post in site.posts %}
## {{post.date | date: "%Y-%m-%d"}}: {{post.title}}
[View]({{ post.url | relative_url }}) {{ post.description }}
{% endfor %}

