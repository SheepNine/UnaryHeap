---
title: The Unary Heap
layout: baremetal-page
---

# Recipes

{% assign blah = site.collections | where: "label", "recipes" | first %}

{% for xyzzy in blah.docs %}
  [{{ xyzzy.title }}]({{ xyzzy.url | relative_url }})
{% endfor %}

# Etc

[TODO]({{ site.baseurl }}{% link pages/todo.md %})



{% for post in site.posts %}
## {{post.date | date: "%Y-%m-%d"}}: {{post.title}}
[View]({{ post.url | relative_url }}) {{ post.description }}
{% endfor %}

