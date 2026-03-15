# Readme Linker

A lightweight Unity Editor addon for linking to documentation from within the Project window.  
Link to either a local Markdown file or a url to an external source.

When configured, a clickable icon appears next to the folder in the Project window.

---

## Usage

### 1. Internal Readme

Create a `Readme~` folder anywhere inside your project and place a `Readme.md` file inside it.

```
YourFolder/
└── Readme~/
    └── Readme.md
```

> The `~` suffix tells Unity to exclude the folder from builds.

Readme Linker will detect this automatically and display a **?** icon next to `YourFolder`. Clicking it opens the file in your configured external editor.

---

### 2. External Link

Create a `Readme~` folder anywhere inside your project and place a `link.txt` file inside it containing just the URL.

```
YourFolder/
└── Readme~/
    └── link.txt
```

Readme Linker will detect this automatically and display a **↗** icon next to `YourFolder`. Clicking it will open the URL in your default browser.

> The `.txt` file is used for simplicity across Linux, Mac and Windows.

---

## Examples

Example setups for both methods can be found in the `Examples` folder included with this package.