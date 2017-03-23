//inspired by http://stackoverflow.com/questions/4663710/how-do-i-implement-autocomplete-without-using-a-dropdownlist
$(function () {

    var allData = [
[
  'show',
  'show me',
  'view',
  'display',
  [
      'donor [name or email or phoneNumber]',
      'account [title]'
  ]
],
[
  'show',
  'show me',
  'view',
  'display',
  [
      'all',
      '*',
      [
          'donors',
          'open batches [search]',
          'closed batches [search]',
          'batches [search]',
          'batch [search]',
          'donations [value, donor name, account or search]',
          'accounts'
      ]
  ]
],
[
  'edit',
  'modify',
  [
      '[donor name or search]'
  ]
],
[
  'add',
  'add new',
  'create',
  'create new',
  'new',
  'make',
  [
      '[donor name, email or phoneNumber]',
      'donor [donor name, email or phoneNumber]',
      'donation [donation name]',
      'batch [batch name]',
      'account'
  ]
],
[
  'close',
  'post',
  [
      'batch',
      [
          '[batch name]'
      ]
  ]
],
[
  'help',
  'sos',
  'help me',
  'list of commands'
],
[
  'print',
  'make',
  [
      'receipts'
  ]
],
[
    'toggle',
    [
        'showing',
        'viewing',
        'displaying',
        [
            'open',
            'closed',
            'posted',
            [
                'batches'
            ]
        ]
    ]
]
    ];
    var haystack = [];
    for (outer = 0; outer < allData.length; outer++) {
        input = allData[outer];
        var tempArray = [];
        var tempArray2 = [];
        do {
            //for every element in the input
            for (i = 0; i < input.length; i++) {
                if (Array.isArray(input[i])) {
                    break;
                }
                if (tempArray2.length == 0 && !Array.isArray(input[i])) {
                    if (input[i] != "*") {
                        tempArray.push(input[i]);
                    } else {
                        tempArray.push("");
                    }

                } else {
                    //append
                    for (j = 0; j < tempArray2.length; j++) {
                        if (!Array.isArray(input[i])) {
                            if (input[i] != "*") {
                                if (tempArray2[j].length == 0) {
                                    tempArray.push(input[i]);
                                } else {
                                    tempArray.push(tempArray2[j] + " " + input[i]);
                                }

                            } else {
                                tempArray.push(tempArray2[j]);
                            }
                        }
                    }
                }
            }
            //if the last one is not an array, break out
            if (!Array.isArray(input[input.length - 1])) {
                break;
            }
            input = input[input.length - 1];
            tempArray2 = tempArray;
            tempArray = [];
        }
        while (true);
        $.merge(haystack, tempArray);
    }
    console.log(haystack);

    //use the KEYDOWN to cancel out the tab key in the #search interface
    $('#mainInput').keydown(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code == 9) {
            return false;
        }
    });

    //act on the keyup
    $('#mainInput').keyup(function (e) {
        var $suggest = $('#suggestion');
        var code = (e.keyCode ? e.keyCode : e.which);

        // 'tab' key was pressed
        if (code == 9) {
            s = $suggest.val();

            //if there is a [] somewhere in the suggestion, jump to it, otherwise fill the box
            if (s.indexOf('[') > -1) {
                s = s.substring(0, s.indexOf('['));
            }

            //if there is no suggestion, break
            if (s == "") {
                return false;
            }
            $(this).val(s);

            return false;
        }

        // if some other key was pressed
        var needle = $(this).val();
        // is the field is empty, make the suggestion ""
        if (!$.trim(needle).length) {
            $suggest.val("");
            return false;
        }

        // compare input with haystack to get a suggestion
        $.each(haystack, function (i, term) {
            var regex = new RegExp('^' + needle, 'i');
            if (regex.test(term)) {
                $suggest.val(needle + term.slice(needle.length));
                // use first result
                return false;
            }
            $suggest.val("");
        });
    });

});