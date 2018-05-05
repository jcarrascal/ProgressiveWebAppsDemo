# Progressive Web Applications Demo
A Progressive Web Application needs to have responsive design, that is, it acomodates it self to the size of the screen it is being shown. We are going divide the UI controls into four groups and move them around depending on the screen size.

## The Mobile-First design
We start with a design for smartphones because it is easier. It will flow all the blocks in a vertical orientation.

## The design for tablets
On tablet devices we'll have the balance centered on top, the ammount and buttons on the right and the reason and other details on the left with the movements table below.

## The design for computers
For bigger devices well have the first three blocks in a row and the movements table on the bottom.

For achieving this design we used Bootstrap classes, a CSS rule and a Boostrap theme. You'll find the Bootstrap theme on /wwwroot/lib/bootstrap/dist/css/bootstrap.theme.css. This theme was downloaded from https://bootswatch.com/

The CSS class just makes sure the blocks have the same height when they are side by side.

```css
@media (min-width: 750px) {
    body {
        padding: 30px 0;
    }
    .row {
        display: flex;
        flex-wrap: wrap;
    }
    .row > .panel {
        display: flex;
        flex-direction: column;
        justify-content: center;
    }
}
```
