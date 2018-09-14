# Get started

https://safe-stack.github.io/docs/quickstart/#create-your-first-safe-app

1) Install fake as global tool (``dotnet tool install fake-cli -g``
2) ``fake build --target run``

This should restore everything, build it and eventuall open a browser.

# What did I do?

Take a look at the commit messages.

```
dotnet new -i SAFE.Template
dotnet new SAFE

yarn add awesome-typescript-loader --dev
yarn add typescript --dev
yarn add babylonjs
yarn add @types/react --dev
```

I just copy-pasted any ``tsconfig.json``, you may want to tweak that a bit.

Integrating ATS into Webpack took only two changes:


```f#
module.exports = {

    resolve: {
    
        extensions: ['.ts', '.tsx', '.wasm', '.mjs', '.js', '.json'], // default is ['.wasm', '.mjs', '.js', '.json']
        
    },
        
        
    module: {
        rules: [
        
            {
                test: /\.tsx?$/,
                loader: 'awesome-typescript-loader'
            }
            
        ]
    },
```

