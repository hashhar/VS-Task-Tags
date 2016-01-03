# Contribution Guidelines

## Branching Model

The repository follows the [following]( http://nvie.com/posts/a-succesful-git-branching-model) branching model:

- `master` - Contains the latest shipping version. Must always be in a build ready state.
- `develop` - The development area. All the work happens here. **If you plan a pull request, please branch off this and not `master`.**
- `release` - The builds ready for release are on this branch. It branches from `develop` and is a temporary branch only used to test the build. All last minute bugfixes are done on this branch.
- `feature-branch` - Branches that are working to implement a specific feature. This is where the actual work happens. **These branch off from `develop` NOT `master`.**

## Coding Standards

- I am a huge proponent of "Tabs for indentation and spaces for alignment" idea. (Believe me when I say you will never want to go back to any other style).
```
if (x > 0)
    return 1;   // \t
else
{
    CreateNewExample(param1,    // \t
                     param2,    // \t followed by required number of spaces
                     param3)    // \t followed by required number of spaces
    if (x == 0)                 // \t
        DeleteNewStuff(param1,  // \t\t
                       param2,  // \t\t followed by required number of spaces
                       param3)  // \t\t followed by required number of spaces
}
```
The basic idea is to use tabs to bring text to the correct indentation level, and use spaces to align text.
- Braces must always start on a new line.
- Single line `if` statements are bad
```
if (condition)
    statement
```
is preferred against
```
if (condition) statement
```
