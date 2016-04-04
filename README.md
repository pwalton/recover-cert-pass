# recover-cert-pass
Attempts to recover a lost certificate password with a "dictionary attack"

## How to use this
In `App.config`, adjust `appSettings` according to the information below.

* `CertificatePath` - path of the certificate you would like to test passwords against
* `DictionaryPath` - path of the file containing dictionary items you would like to use to generate passwords
* `DictionaryPrefixesToSkip` - path of the file containing dictionary items you do not want to be used as prefixes 
* `DictionaryItemsMayRepeat` - bool indicating whether or not words in your dictionary might repeat
* `MinimumPasswordLength` - the minimum length a password should be before it is tested against the cert
* `MaximumPasswordLength` - the maximum length a password may be if it is to be tested against the cert
* `MaximumDictinaryItemsToUse` - the maximum number of words to use from the dictionary when creating passwords

### Tips
##### Dictionary
The program works through the dictionary items in order. Having the most likely prefixes near the beginning of the dictionary will be beneficial, especially if the potential password is comprised of more than 4 words from the dictionary.

Try to make the words as "complete" as possible. If you never type _lorem_ without typing _ipsum_ after it, then list _loremipsum_ as a single entry in the dictionary. If possible, compose the dictionary in such a way that 4 or fewer words from it would be necessary to compose a potential password.

The larger your dictionary, the longer each pass will take, so it is worth starting with a short dictionary, increasing it in size only as your desperation increases. :)

##### Prefixes to skip
If you never type _ipsum_ without ever typing _lorem_ before it, it probably is not worth considering any password starting with _ipsum_. List the dictionary items you do not want to use as prefixes in your `DictionaryPrefixesToSkip` file. They will still be used, just not as the first word of your password.

##### Repeating words
If _lorem_ is in your dictionary, and if it is possible that the password is _loremipsumlorem_, you would set `DictionaryItemsMayRepeat` to `true`. When set to false, dictionary items will not be repeated in a password.

##### Password length
If you know precisely how long the password is, or just have a rough idea, it is worthwhile to set the minimum and maximum password length accordingly. Using a wider range simply means the program will test more passwords. Not the end of the world.

##### Maximum number words to use
This indicates to the program when it is time to throw its hands up. If your password might have more than 4 items from your dictionary, it might be worth spending some time improving your dictionary.

### Testing it out
There are some values already in `App.config` and some files in the `Resources` folder that will let you see the program in action. The password for `certycert.p12` is _`testingLoremipsum@evil.com`_. I encourage you to mess with the dictionary or the `appSettings` to see how it affects performance.

-----
## The why
I had forgotten a password for an important p12 cert file. Normally, I would have recorded the password or would have made it easily memorable. In the hope that I had used what I considered a memorable password, I came up with a dictionary of words I might have used, including a couple of variants on each word in case I might have fat-fingered one of the words or accidentally left capslock on. Fortunately, in this specific case, I knew the password had to be of a specific length, so I wrote a program to go through each combination of the words in the dictionary, testing each combination when at the appropriate length.

## Mistakes I made along the way
The first version of the program went through each word in the dictionary, creating every combination that began with that word. For example, if my dictionary consisted of the first 50 words of _lorem ipsum_, it would first go through every single password that started with _lorem_, then move on to _ipsum_, and so forth. This approach is notably slow, especially if the password I was looking for started with _vehicula_, the 45th word in this hypothetical dictionary.

I decided to have multiple threads churning out passwords simultaneously, so the next version had four threads, each of which would concentrate on a single word. It was considerably faster than a single threaded application doing the same job, but it had the same shortfalls as the first approach: if the prefix of your password is near the end of the list, you will be waiting awhile. Consequently, the multithreaded version found my password - it took six hours.

While I was happy with having found the lost password, I believed the speed of the process could be improved. I rewrote the program several times, each time doggedly focusing on a multithreaded solution. 

The one thing I found during this process that reliably increased the speed of the process was to include a list of words that should never be used as a password prefix. For example, I might start a password with _lorem_ or _ipsum_, but never _dolor_, _sit_, or _amet_.

## Cartesian products make things way better
Despite my best efforts at implementing a dictionary attack that relied on multithreading, I pretty consistently ran into problems with the threads interfering with each other. In an effort to mitigate this, I started from scratch, breaking the behavior of the program out into single responsibility classes. I would need a class that generated passwords, a class that tested passwords, and a class that managed the input and output of the two previous classes.

During this process I discovered a much more efficient means of finding my lost password. Instead of generating every conceivable password that begins with _lorem_ before moving on to _ipsum_, the program would look at the [Cartesian product](https://en.wikipedia.org/wiki/Cartesian_product) of each item in the dictionary with each item previously tested.

For example, if the dictionary was composed of {a, b}, the first pass would test {a, b}, the second pass would test {aa, ab, ba, bb}, the third pass would test {aaa, aba, aab, abb, baa, bba, bab, bbb}, and so forth. In my case, my lost password was comprised of two words in my dictionary, so the new program found my password on it's second pass, finding in one second what the first version of the program took 6 hours to find.

