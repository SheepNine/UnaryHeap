---
layout: baremetal
---

This is the first post of the second incarnation of the Unary Heap. It has been freed of the shackles of Blogspot, to breathe the rarefied air of Github Pages.

As a developer, I like the idea of having my blog be something that can basically self-host itself out of a repository that I control. I had to emigrate from CodePlex a while back because it was flaking out, going for days at a time without access and making me nervous as hell. I put it in Github because that is the fashionable thing to do these days. Once I found out that Github Pages was a thing, it was only a matter of time before content migrated here.

The question remained, though. What should I write about in the inagural post? I needed something with more that one sentence of content, to test that the layout I was developing would look good with the copy that I write. The answer: I will write about the layout itself, and document some design decisions.

So!

## Zero to sixty

The selection of colors, fonts, and sizes is not something that I am generally good at or enjoy. Readers of the old Blogspot blog may have noticed that it was the default layout. This was intentional. It looked good enough for me. If I made any customizations at all (and honestly, it was so long ago that I don't even remember), it was to remove superfluous elements. Actually, I do remember: I tore out a bunch of garbage for Google Plus integrations.

This time, I decided to start from literally scratch (modulo the fact that I am using Jekyll, to be a good Github Pages citizen). Inspired by [this site (NSFW)](http://motherfuckingwebsite.com), I started with no CSS at all, and tried out the default user agent stylesheet. Bleh. Now I know why people have been adding styling information to their HTML since HTML got styling information.

I was then inspired by [this site (also NSFW)](http://bettermotherfuckingwebsite.com), and gave a go at a small amount of CSS to pretty it up, but remain minimalist. The results were better, but not really what I wanted. But what *did* I want?

## Terminal echo

A blog is an intensely personal exercise, and so I resolved to pick a personal style. What do I like, from a typographic standpoint? The answer became clear when I realized that I spend most of my days staring at terminal windows and code editors: monospace fonts. They give me the precise, anal-retentive control I need over exactly positioning everything. The rest of the design for the blog fell out of that decision: I would stylize my blog as a terminal window.

How many characters wide would I make the window? Tradition demands 80 characters, as does some layout wisdom floating around on the internet today. I instead opted for 120 characters, chiefly out of the desire to give myself enough room to add larger images if I so chose.

Here I learned my first web design gem. The CSS font-size declaration is for the height of your text. The width of your text is a function of that height and your particular font's aspect ratio.

This was a problem for me. Different browsers pick different fonts when you specify 'monospace' as the font, and of freaking course, they have different aspect ratios. I went with Consolas originally, because I am at heart a Windows developer, but gave up on it after I discovered that Consolas on a mac looks like trash because [the font is buggy and its baseline is wrong](http://mbauman.net/geek/2009/03/15/minor-truetype-font-editing-on-a-mac/).

I went with Ubunto Mono, for two reasons. First, I can get it free from Google APIs, so I guarantee that my blog looks the same everwhere. Second, its aspect ratio turns out to be exactly 1:2. This makes me very pleased.

For the font size, I punted, and went with the browser-default 16px. It is a power of two, and it meant that at my chosen text width, I can fit two windows side-by-side on my widescreen monitor, for those times that I just have to read two different blog posts I wrote simultaneousely.

16 x 120 x 0.5 = 960px wide body. Done.

## Color your world

My first impulse was to not use any color at all: monospace font with monochromatic palette, like a true terminal window. I abandoned this idea after struggling to identify links in the text without a color hint.

If I couldn't use mono, then CGA was the next best thing. I picked a few colors and tried it out. I was happy with the results. My fiance, however, was not. It seemed I had chosen too harsh a contrast, and so we worked together to pick the final colors: dark teal on black. It has enough contrast to be readable without being harsh on the eyes.

Thanks sweets!

## Devil in the details

My next step was all those important text formatting that a blog needs: headers, emphasis, links, and the like. Here, I gave myself an artificially-imposed limitation: I would not use bold or italic font. That would be cheating in a terminal window. (Now you see why color became so important). Instead, with the help of CSS :before and :after selectors, I just surround my formatted text with the same characters that appeared in old internet posts: *underscores* for emphasis, and **asterisks** for strong emphasis. For links, I deferred to [Kramdown](https://kramdown.gettalong.org/), where square brackets are the norm.

For headers, I used the same selectors, and added a few dashes or equals-signs and a splash of color.

I haven't yet solved for lists or tables. All in due time.

## Mobile-friendly

I like my layout, but I also wanted the blog to be something that can be read comfortably on a mobile device, since that is the way the world is going. Adding a viewport meta and some media queries to my site made it so that it would drop the page background and switch from a fixed-width to a full-width layout once the viewport fell below its desired width.

Here, I ran into my second web design gem. The viewport width either does, or does not, include the scrollbar. Thanks, browser vendors! It required a bit of hacking around to get the resizing behaviour I desired, but it is done.

Testing on my LG G3 reveals that my chosen styles will render at 74 characters per row (landscape mode) or 55 characters per row (portrait mode). No pixels are wasted.

## Lightweight

Another decision I made to make the site mobile-friendly was to eschew all forms of javascript. You won't find a comment box, or a Facebook like button, or anything else anywhere on this site. Such a decision is anathema to most websites today, which depend critically on 'user engagement' tracked by some such 'deep data analytics' or other. I am not so vain as to assume I have any readers besides myself.

## Sizzle versus Steak

Ultimately, a blog is only as interesting as its content. My next post will be on some other topic besides the blog itself. Hopefully this post has given you a better appreciation for the design of all websites, not just mine. On nearly every one you will see, someone laboured over the choice of colors and fonts. Maybe it gave you some ideas for a design of your own.
