---
title: The Unary Heap
layout: baremetal-page
---

{% for post in site.posts %}
## {{post.date | date: "%Y-%m-%d"}}: {{post.title}}
[View]({{post.url | relative_url}}) {{ post.description }}
{% endfor %}

