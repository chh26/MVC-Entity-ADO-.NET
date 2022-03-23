var path = require('path'),
  webpack = require('webpack'),
  node_modules_dir = __dirname + '/node_modules',
  plugins = [],
  outputfile = '[name].js';

require("babel-polyfill");
if (0 <= process.argv.indexOf("--min")) { // compress
    var UglifyJSPlugin = require('uglify-es-webpack-plugin');

    plugins.push(new UglifyJSPlugin({
        compress: { warnings: false, drop_console: true, dead_code: true }
    }));
}
module.exports = {
    /* 壓縮檔案進入點 */
    entry: {
        XQLiteMgm: ['babel-polyfill', path.resolve(__dirname, 'src/XQLiteMgm.jsx')],
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: outputfile,
        publicPath: './dist/'
    },
    /*外部套件 */
    plugins: plugins,
    resolve: { alias: {} },
    /*編碼器 */
    module: {
        loaders: [
          {
              test: /\.js[x]?$/,
              exclude: /(node_modules|bower_components)/,
              // 'babel-loader' is also a legal name to reference
              loader: 'babel-loader',
              query: {
                  compact: false,
                  cacheDirectory: true,
                  presets: ['env', 'stage-0', 'react'],
                  plugins: [["import", { libraryName: "antd", style: "css" }]]
              }
          },
          { test: /\.css$/, loader: "style-loader!css-loader" },
          { test: /\.less$/, loader: "style-loader!css-loader!less-loader" },
          // inline base64 URLs for <=8k images, direct URLs for the rest
          {
              test: /\.(png|jpg|gif)$/,
              loader: 'url-loader?name=[path][name].[ext]&limit=50000'
          },
          { test: /\.eot(\?v=\d+\.\d+\.\d+)?$/, loader: "file-loader?name=[path][name].[ext]" },
          {
              test: /\.woff(2)?(\?v=[0-9].[0-9].[0-9])?$/,
              loader: "url-loader?mimetype=application/font-woff"
          },
          {
              test: /\.ttf(\?v=\d+\.\d+\.\d+)?$/,
              loader: "url-loader?name=[path][name].[ext]&limit=50000&mimetype=application/octet-stream"
          },
          {
              test: /\.svg(\?v=\d+\.\d+\.\d+)?$/,
              loader: "url-loader?name=[path][name].[ext]&limit=50000&mimetype=image/svg+xml"
          }
        ],
        noParse: [/moment-with-locales/]
    }
};
