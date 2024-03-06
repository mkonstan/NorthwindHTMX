/// <binding BeforeBuild='default' ProjectOpened='watch:css, watch:js' />
module.exports = function (grunt) {
    grunt.initConfig({
        "pkg": grunt.file.readJSON("package.json"),
        "babel": {
            dist: {
                options: {
                    sourceMaps: false
                },
                files: [
                    {
                        expand: true,
                        cwd: "wwwroot",
                        src: ["*.jsx", "**/*.jsx"],
                        dest: "wwwroot",
                        ext: ".js",
                        extDot: "first"
                    }
                ]
            },
            prod: {
                options: {
                    sourceMaps: false,
                    minified: true,
                    compact: true
                },
                files: [
                    {
                        expand: true,
                        cwd: "wwwroot",
                        src: ["*.jsx", "**/*.jsx"],
                        dest: "wwwroot",
                        ext: ".min.js",
                        extDot: "first"
                    }
                ]
            }
        },
        "sass": {
            options: {
                implementation: require("sass"),
                sourceMap: false
            },
            dist: {
                files: [
                    {
                        expand: true,
                        cwd: "wwwroot/css",
                        src: ["*.scss", "**/*.scss"],
                        dest: "wwwroot/css",
                        ext: ".css",
                        extDot: "first"
                    }
                ]
            }
        },
        "watch": {
            js: {
                files: ["wwwroot/**/*.jsx"],
                tasks: ["babel"]
            },
            css: {
                files: ["wwwroot/**/*.jsx"],
                tasks: ["sass"]
            }
        }
    });

    // Load the plugin that provides the "uglify" task.
    grunt.loadNpmTasks("grunt-contrib-concat");
    grunt.loadNpmTasks("grunt-babel");
    grunt.loadNpmTasks("grunt-sass");
    grunt.loadNpmTasks("grunt-contrib-watch");

    // Default task(s).
    grunt.registerTask("default", ["babel", "sass"]);
};
